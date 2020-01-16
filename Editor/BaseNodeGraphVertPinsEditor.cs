﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseNodeGraphVertPinsEditor<T> : BaseNodeGraphEditor<T> where T : BaseDialogueGraphVertPinData
{

	public BaseNodeGraphVertPinsEditor ( int uid ) : base( uid ) { }
	public BaseNodeGraphVertPinsEditor ( int uid, Rect pannelRect ) : base( uid, pannelRect ) { }

	protected override Vector2 NodeSize ()
	{
		return new Vector2( 300, 150 );
	}

	protected override Vector2 NodeSize ( int nodeId )
	{
		Vector2 nodeSize = NodeSize();

		return nodeSize;
	}

	protected override Rect GetNodeContentsPosition ( int nodeId )
	{
		Vector2 nodeSize = nodes[ nodeId ].NodeRect.size;

		return new Rect()
		{
			y = 0,
			x = 22,
			width = nodeSize.x - 10,
			height = nodeSize.y - 44
		};
	}

}

public class BaseDialogueGraphVertPinData : BaseNodeGraphData
{

	public BaseDialogueGraphVertPinData ( string _title, bool _dragable ) : base( "", _dragable ) { }
	public BaseDialogueGraphVertPinData ( string _title, bool _dragable, Vector2 _inputStartPosition, Vector2 _outputStartPosition, Vector2 _pinSize ) :
		base( "", _dragable, _inputStartPosition, _outputStartPosition, _pinSize )
	{ }


	public override NodePin_Output.BezierControlePointOffset BezierControlePointOffset { get => NodePin_Output.BezierControlePointOffset.Vertical; }

	public override void GeneratePinSizeAndPosition ( Vector2 nodeSize )
	{
		pinSize.x = ( nodeSize.x / Mathf.Clamp( NodeConnections_output.Count, 1, 4 ) ) - ( 50 / Mathf.Clamp( NodeConnections_output.Count, 1, 4 ) ) - 12f;

		inputPin_localStartPosition.x = -pinSize.x + 25;
		inputPin_localStartPosition.y = 0;

		outputPin_localStartPosition.x = -pinSize.x + 25;
		outputPin_localStartPosition.y = nodeSize.y - ( pinSize.y * 2f );

	}

	protected override Vector2 GetPinOffset ( int pinId, PinMode pinMode )
	{
		Vector2 pinOffset = Vector2.zero;

		switch ( pinMode )
		{
			case PinMode.Input:
			pinOffset.x = pinSize.x;
			break;
			case PinMode.Output:
			pinOffset = pinSize;
			pinOffset.x += ( pinSize.x + 26 ) * pinId;
			break;
		}

		return pinOffset;

	}

	public override Vector2 GetConnectionOffset ( PinMode pinMode )
	{
		switch ( pinMode )
		{
			case PinMode.Input:
			return new Vector2( pinSize.x / 2f, 0 );
			case PinMode.Output:
			return new Vector2( pinSize.x / 2f, pinSize.y );
		}

		return Vector2.zero;
	}

}