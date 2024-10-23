using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Direction handles the direction of the pipe in the pipe game. This game
 * works by making sure that whatever is flowing through the pipes can only
 * flow in one direction.
 * 
 * For straight pipes, this is rather straight forward, if the pipe is up, then
 * it's in a shape like | and it flows upwards.
 *
 * Turn pipes are a little different. A diagram is drawn below. The | and --
 * indicate the pipe itself, while arrows such as ->, <-,
 * ^      |
 * |, and v, indicate flow.
 * 
 * turnPipe up
 *          |--   ->
 *          |
 *          
 *          ^
 *          |
 *          
 * turnpike right
 * 
 *  ->   --|
 *         |
 *         
 *         |
 *         v
 *         
 * turnpipe down
 * 
 *           |
 *           v
 *         
 *           |
 *   <-    --|
 *   
 * turnpipe left
 * 
 *        ^
 *        |
 * 
 *        |
 *        |--  <-
 *        
 *         
 * Notice that the flow starts in the direction of the Direction, and ends 90 deg clockwise
 * 
 * For source and sink nodes, it is also a little different
 * For source nodes, the direction will be where the water must flow and it will come out of the bottom of the triangle
 * For sink nodes, the direction will be where the water must flow into and it will flow into the bottom of the triangle
 *
 * Down direction for sink node
 *
 *         |
 *         v 
 *
 *       ----        
 *       \  /
 *        \/
 *        
 * Down direction for source node
 *
 *        /\
 *       /  \
 *       ----
 *
 *        |
 *        v
 *
 */
public enum Direction
{
    up,
    right,
    down,
    left
}

/**
 * PipeType determines if the type is a straight pipe, turn pipe, source, or sink
 * @see Direction
 */
public enum PipeType
{
    straight,
    turn,
    source,
    sink,
    empty
}

// FIXME: write javadoc string here
public class PipeInfo
{
    public Direction direction;
    public PipeType type;

    public PipeInfo(Direction d, PipeType pt)
    {
        direction = d;
        type = pt;
    }
}
