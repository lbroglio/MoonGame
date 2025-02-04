using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    /// <summary>
    /// The max speed of the player
    /// </summary> 
    public float MaxSpeed = 10;

    /// <summary>
    /// Speed at which the camera rotates
    /// </summary> 
    public float RotateSpeed = 1;

    /// <summary>
    /// The (absolute value of) maximum amount the player can rotate the camera in the y direction
    /// </summary>
    public float MaxXRotation = 90;

    // Direction of the players forward movement
    private Vector2 _moveVector = Vector2.zero;

    // The current speed the player is moving at along each axis
    private Vector2 _currSpeed;

    /// <summary>
    /// How high the player can jump
    /// </summary>
    public float JumpHeight = 1;

    /// <summary>
    /// How high the player moves up when stepping
    /// </summary>
    public float StepHeight = 1;

    // How long the player takes to reach the maximum jump height (In seconds)
    // Calulated based on gravity
    private float _jumpTime;

    // Track how much the player has jumped 
    private float _currJumpHeight = 0;

    // Track how much the player has jumped 
    private float _currStepHeight = 0;

    // Track if the player is currently jumping
    private bool _jumping = false;

    // Track if the player is currently stepping
    private bool _stepping = false;

    // The current amount the camera is rotated in the x direction (In degrees)
    private float _currXRot = 0;

    // The current amount the camera is rotated in the y direction (In degrees)
    private float _currYRot = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate jump time based on gravity 
        JumpHeight *= GameObject.Find("WorldManager").GetComponent<Init>().GravityDivideFactor;
        _jumpTime = Math.Abs(JumpHeight / Physics.gravity.y);
    }

    // Update is called once per frame
    void Update()
    {

        // Decide how to move based on current state and input

        // If the player is accelerating forward
        bool playerGrounded = Utils.IsGrounded(gameObject);
        // Forward / Back axis
        if(playerGrounded){
            // Stop any current movement
            _currSpeed.x = 0;
            _currSpeed.y = 0;

            // Push off in a given direction
            if(Input.GetKey(KeyCode.W)){
                _currSpeed.y = MaxSpeed;
                _stepping = true;
                _currStepHeight = 0;
            }
            else if(Input.GetKey(KeyCode.S)){
                _currSpeed.y = -1 * MaxSpeed;
                _stepping = true;
                _currStepHeight = 0;
            }
            
            if(Input.GetKey(KeyCode.A)){
                _currSpeed.x = -1 * MaxSpeed;
                _stepping = true;
                _currStepHeight = 0;
            }
            else if(Input.GetKey(KeyCode.D)){
                _currSpeed.x = MaxSpeed;
                _stepping = true;
                _currStepHeight = 0;
            }
            
        }

        // Step logic logic
        float frameStep = 0;
        if(_stepping){
            // If at the top of the jump stop jumping
            if(_currStepHeight >= StepHeight){
                // Debug.Log(_currJumpHeight);
                _stepping = false;
            }
            // Otherwise set increase for this frame
            else{
                frameStep = StepHeight + (-1 * Physics.gravity.y);
                _currStepHeight += frameStep * Time.deltaTime;
            }
        }

     
        // Jumping logic
        float frameJump = 0;
        if(_jumping){
            // If at the top of the jump stop jumping
            if(_currJumpHeight >= JumpHeight){
                // Debug.Log(_currJumpHeight);
                _jumping = false;
            }
            // Otherwise set increase for this frame
            else{
                frameJump = JumpHeight + (-1 * Physics.gravity.y);
                _currJumpHeight += frameJump * Time.deltaTime;
            }
        }

        // If the jump key is pressed and the player is on the ground
        if(Input.GetKey(KeyCode.Space) && playerGrounded){
            _jumping = true;
            _currJumpHeight = 0;
        }

        // TODO: This currently rotates the camera it should be changed to the models head (Once a model is added)
        // Rotate based on mouse
        float rotYDelta = RotateSpeed * Input.GetAxis("Mouse X");
        float rotXDelta = -1 * RotateSpeed * Input.GetAxis("Mouse Y");
        if(Math.Abs(_currXRot + rotXDelta) < MaxXRotation){
            _currXRot += rotXDelta;
        }
        else{
            _currXRot = MaxXRotation * (_currXRot > 0 ? 1 : -1);
        }
        _currYRot = (_currYRot + rotYDelta) % 360;

        // Apply rotation to camera
        Quaternion newRot = Quaternion.Euler(new Vector3(_currXRot, _currYRot, 0));
        Transform cam = transform.Find("Main Camera");
        cam.rotation = newRot;



        // Move player by the current speed
        Vector3 moveVector = new Vector3(_currSpeed.x, frameJump + frameStep, _currSpeed.y);
        // Debug.Log(moveVector);
        moveVector = Matrix4x4.Rotate(Quaternion.Euler(0, cam.transform.eulerAngles.y, 0)) * moveVector;
        transform.position += Time.deltaTime * moveVector;

    }
}