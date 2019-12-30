﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNodeEditor<T> where T : BaseNodeData
{
    public delegate void nodeSelected ( int winId );
    public event nodeSelected nodePressed;
    public event nodeSelected nodeReleased;

    int uniqueID;

    public Rect panelRect { get; set; }
    public virtual Vector2 topLeftpadding { get => new Vector2( 18, 18 ); }
    public virtual Vector2 bottomRightpadding { get => new Vector2( 18, 18 ); }

    protected Vector2 panelScrollPosition;

    protected List<T> nodes;
    protected int pressedNode = -1; // < 0 == none
    protected int releasedNode = -1; // < 0 == none

    Vector2 lastScrolBarPosition = Vector2.zero;

    public BaseNodeEditor (int uid)
    {
        uniqueID = uid * 1000;
        nodes = new List<T>();
    }

    public BaseNodeEditor (int uid, Rect holdRect) : this (uid)
    {
        panelRect = holdRect;
    }

    public virtual void Update () { }

    /// <summary>
    /// Draws the node window to editorWindow.
    /// </summary>
    /// <param name="window"></param>
    public virtual void Draw ( EditorWindow window ) 
    {

        // Draw background box
        Vector3[] v = { panelRect.position - topLeftpadding,   panelRect.position + new Vector2( panelRect.size.x + bottomRightpadding.x, -topLeftpadding.y ),
                        panelRect.position + panelRect.size + bottomRightpadding, panelRect.position + new Vector2(-topLeftpadding.x, panelRect.size.y + bottomRightpadding.y) };
        Handles.DrawSolidRectangleWithOutline( v, new Color(0.8f, 0.8f, 0.8f), Color.gray );

        // get scroll position
        Rect scrollRect = panelRect;
        scrollRect.size += new Vector2( 18, 18 );

        panelScrollPosition = GUI.BeginScrollView( scrollRect, panelScrollPosition, GetPannelViewRect() );
        GUI.EndScrollView();

        Vector2 scrolDelta = panelScrollPosition - lastScrolBarPosition;
        scrolDelta = -scrolDelta;

        // Fix nodes not calling release when cursor leaves window ( Note: we dont get the event if we do this in Update :| )
        // release the pressed node preventing it from geting drawn for one update so the cursor no longer has focus of the node
        // then trigger a repaint at the end to trigger the released node event
        if ( pressedNode > -1 && Event.current != null && !PositionIsVisable( Event.current.mousePosition  ) )
        {
            // make the release the pressed n
            releasedNode = pressedNode;
            pressedNode = -1;
        }
        else if( releasedNode > -1 )
        {
            // call the released node event
            nodeReleased?.Invoke( releasedNode ); 
            releasedNode = -1;
        }

        // draw nodes if visable
        for ( int i = 0; i < nodes.Count; i++ )
        {
            nodes[ i ].MoveNode(scrolDelta);

            // hide node if not viable of if the node has been releassed due to the mouse leaveing the node area.
            if ( (!PositionIsVisable( nodes[ i ].GetCenter() ) && pressedNode != i ) || (pressedNode < 0 && releasedNode > -1) )
                continue; 

            DrawNode( i );
            
        }
        

        lastScrolBarPosition = panelScrollPosition;

        // trigger repaint if node has been released.
        if ( releasedNode > -1 )
            window.Repaint();

    }

    protected virtual void DrawNode( int nodeId )
    {
        nodes[ nodeId ].NodeRect = GUI.Window( uniqueID + nodeId, nodes[ nodeId ].NodeRect, NodeWindow, nodes[ nodeId ].title );
        nodes[ nodeId ].NodeRect = ClampNodePosition( nodes[ nodeId ].NodeRect, nodeId );
    }

    /// <summary>
    /// The viewable area within the pannel. if larger than pannel rect scroll bars will be added :D
    /// </summary>
    /// <returns></returns>
    protected virtual Rect GetPannelViewRect()
    {
        return new Rect(Vector2.zero, panelRect.size*2);
    }

    protected virtual Vector2 GetPanelOffset()
    {
        return panelRect.position + panelScrollPosition;
    }

    /// <summary>
    /// defines how a node should be sized.
    /// </summary>
    /// <returns></returns>
    protected abstract Vector2 NodeSize ();

    /// <summary>
    /// Defines where a node should be spawned
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 NodeStartPosition()
    {
        return panelRect.position;
    }

    /// <summary>
    /// Check if position is visable within the scroll view
    /// </summary>
    /// <param name="position">position in Editor Window</param>
    protected bool PositionIsVisable ( Vector2 position )
    {
        //position -= panelRect.position;

        return !(position.x < panelRect.x || position.x > panelRect.x + panelRect.width ||
               position.y < panelRect.y || position.y > panelRect.y + panelRect.height);

    }

    /// <summary>
    /// Gets node at id
    /// </summary>
    /// <returns></returns>
    public virtual T GetNode ( int id )
    {
        if ( id < 0 || id >= nodes.Count ) return null;
        return nodes[ id ];
    }
    /// <summary>
    /// Adds a new node
    /// </summary>
    public abstract T AddNode ( string title, bool isDragable );

    public abstract T AddNode ( T data );

    /// <summary>
    /// removes nodes of nodeData
    /// </summary>
    public virtual void RemoveNode ( T nodeData )
    {
        nodes.Remove( nodeData );
    }

    /// <summary>
    /// removes node at id
    /// </summary>
    public virtual void RemoveNode (int id)
    {
        if ( id < 0 || id >= nodes.Count ) return;
        nodes.RemoveAt( id );
    }

    /// <summary>
    /// Clamps node rect to postion
    /// </summary>
    /// <param name="nodeRect"></param>
    /// <returns></returns>
    protected abstract Rect ClampNodePosition ( Rect nodeRect, int nodeId = 0 ); // NOTE: winId is only used to fix the issue in node window.

    /// <summary>
    /// draws node data for windowId
    /// </summary>
    /// <param name="winId"></param>
    protected virtual void NodeWindow ( int winId )
    {

        int nodeId = winId - uniqueID;

        // BUG: if the cursor leaves the node when pressed the release is not triggered.
        if ( Event.current.type == EventType.MouseDown )
        {
            nodePressed?.Invoke( nodeId );
            pressedNode = nodeId;
            nodes[ nodeId ].pressedPosition = nodes[ nodeId ].GetNodePosition();
            Debug.Log( nodeId + " Pressed" );
        }
        else if ( Event.current.type == EventType.MouseUp)
        {
            nodeReleased?.Invoke( nodeId );
            pressedNode = -1;
            Debug.Log( nodeId + " Released" );
        }
        


        if ( nodes[nodeId].dragable )
        {
            GUI.DragWindow();
        }

    }

}

public class BaseNodeData
{

    private Rect rect = Rect.zero;
    public Rect NodeRect { get => rect; set => rect = value; }


    public Vector2 pressedPosition = Vector2.zero;
    public string title = "title";
    public bool dragable = true;

    public void SetNodePosition(Vector2 position)
    {
        rect.position = position;
    }

    public void MoveNode(Vector2 amountToMove)
    {
        rect.position += amountToMove;
    }

    public Vector2 GetNodePosition()
    {
        return rect.position;
    }

    public Vector2 GetCenter()
    {
        return rect.center;
    }

    public void SetNodeSize(Vector2 size)
    {
        rect.size = size;
    }

    public Vector2 GetNodeSize()
    {
        return rect.size;
    }


}