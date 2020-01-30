﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Obsolete( "It has no functions any more! Use BaseNodeGraphEditor and use 'BaseDialogueGraphVertPinData' as it minimal T type" )]
public abstract class BaseNodeGraphVertPinsEditor<T> : BaseNodeGraphEditor<T> where T : BaseGraphVertPinData
{

	public BaseNodeGraphVertPinsEditor ( int uid ) : base( uid ) { }
	public BaseNodeGraphVertPinsEditor ( int uid, Rect pannelRect ) : base( uid, pannelRect ) { }

}

public abstract class BaseGraphVertPinData : BaseNodeGraphData
{

	public BaseGraphVertPinData ( string _title, bool _dragable ) : base( "", _dragable ) { }
	public BaseGraphVertPinData ( string _title, bool _dragable, int _max_inputPins, int _max_outputPins ) : 
		base( "", _dragable, _max_inputPins, _max_outputPins ) { }

	public BaseGraphVertPinData ( string _title, bool _dragable, Vector2 _inputStartPosition, Vector2 _outputStartPosition, Vector2 _pinSize ) :
		base( "", _dragable, _inputStartPosition, _outputStartPosition, _pinSize )
	{ }


	public override NodePin_Output.BezierControlePointOffset BezierControlePointOffset { get => NodePin_Output.BezierControlePointOffset.Vertical; }

	protected override void GenerateNodeSize ()
	{
		rect.size = new Vector2( 300, 150 );
	}

	public override void GeneratePinSizeAndPosition ()
	{

		GenerateNodeSize();

		pinSize.x = ( rect.width / Mathf.Clamp( NodeConnections_output.Count, 1, 4 ) ) - ( 50 / Mathf.Clamp( NodeConnections_output.Count, 1, 4 ) ) - 12f;

		inputPin_localStartPosition.x = -pinSize.x + 25;
		inputPin_localStartPosition.y = 0;

		outputPin_localStartPosition.x = -pinSize.x + 25;
		outputPin_localStartPosition.y = rect.height - ( pinSize.y * 2f );

	}

	protected override Rect GetNodeContentsPosition ()
	{
		Vector2 nodeSize = rect.size;

		return new Rect()
		{
			y = 0,
			x = 22,
			width = nodeSize.x - 10,
			height = nodeSize.y - 44
		};

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
