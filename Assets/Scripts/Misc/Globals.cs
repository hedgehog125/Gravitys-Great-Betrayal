using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public static class Globals {
	public static GlobalsController CurrentController;

	public static Player.Player CurrentPlayer;
	public static Gravity CurrentGravityController;
	public static ConstantData CurrentConstants;
	public static PauseController CurrentPauseController;
	public static CameraController CurrentCameraController;
}
