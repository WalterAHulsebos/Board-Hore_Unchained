using Rewired;
using System;
using System.Threading;
using UnityEngine;
using XInputDotNetPure;

public class InputThread : MonoBehaviour
{
	public InputController inputController;

	private bool playerIndexSet;

	private PlayerIndex playerIndex;

	private GamePadState state;

	private GamePadState prevState;

	private bool _threadActive;

	public int refreshRate = 8;

	public int RegularUpdatesPerSecond;

	public int ThreadedUpdatesPerSecond;

	private Thread _updateThread;

	private int _updateCount;

	private float _time;

	private int _lastPos;

	private int _pos;

	private object _threadLock = new object();

	public int _maxLength = 100;

	private InputThread.InputStruct _lastFrameData;

	public InputThread.InputStruct[] inputsIn = new InputThread.InputStruct[100];

	public InputThread.InputStruct[] inputsOut = new InputThread.InputStruct[100];

	public InputFilter leftXFilter = new InputFilter();

	public InputFilter leftYFilter = new InputFilter();

	public InputFilter rightXFilter = new InputFilter();

	public InputFilter rightYFilter = new InputFilter();

	private int count;

	private AutoResetEvent reset;

	private bool directInput;

	private float inputTimer;

	public float inputInterval;

	private float _velxMax;

	private float _velxTemp;

	private float _velyMax;

	private float _velyTemp;

	private float _velxAvg;

	private float _velyAvg;

	private float _tempx;

	private float _tempy;

	private int _lastPosClamp
	{
		get
		{
			if (_lastPos - 1 < _maxLength)
			{
				int num = _lastPos;
			}
			else
			{
				int num1 = _maxLength;
			}
			if (_lastPos - 1 < 0)
			{
				return 0;
			}
			return _lastPos - 1;
		}
	}

	public Vector2 avgVelLastUpdate
	{
		get
		{
			TimeSpan timeSpan;
			_tempx = 0f;
			_tempy = 0f;
			if (_lastPosClamp < 2)
			{
				return Vector2.zero;
			}
			for (int i = 1; i < _lastPosClamp; i++)
			{
				float single = _tempx;
				float single1 = inputsOut[i].rightX - inputsOut[i - 1].rightX;
				timeSpan = TimeSpan.FromTicks(inputsOut[i - 1].time - inputsOut[i].time);
				_tempx = single + single1 / (float)timeSpan.Seconds;
			}
			_velxAvg = _tempx / (float)_lastPosClamp;
			for (int j = 1; j < _lastPosClamp; j++)
			{
				float single2 = _tempx;
				float single3 = inputsOut[j].rightY - inputsOut[j - 1].rightY;
				timeSpan = TimeSpan.FromTicks(inputsOut[j - 1].time - inputsOut[j].time);
				_tempx = single2 + single3 / (float)timeSpan.Seconds;
			}
			_velyAvg = _tempx / (float)_lastPosClamp;
			return new Vector2(_velxAvg, _velyAvg);
		}
	}

	public Vector2 lastPosLeft => new Vector2(inputsOut[_lastPosClamp].leftX, inputsOut[_lastPosClamp].leftY);

	public Vector2 lastPosRight => new Vector2(inputsOut[_lastPosClamp].rightX, inputsOut[_lastPosClamp].rightY);

	public Vector2 maxVelLastUpdateLeft => MaxVelLastUpdate(false);

	public Vector2 maxVelLastUpdateRight => MaxVelLastUpdate(true);

	public float x => inputsOut[_lastPosClamp].rightX;

	public float xdel => inputsOut[_lastPosClamp].rightX - inputsOut[0].rightX;

	public InputThread()
	{
	}

	private void Awake()
	{
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; i++)
			{
				PlayerIndex playerIndex = (PlayerIndex)i;
				if (!GamePad.GetState(playerIndex, GamePadDeadZone.None).IsConnected)
				{
					directInput = true;
				}
				else
				{
					directInput = false;
					this.playerIndex = playerIndex;
					playerIndexSet = true;
				}
			}
		}
		for (int j = 0; j < (int)inputsIn.Length; j++)
		{
			inputsIn[j] = new InputThread.InputStruct();
		}
		for (int k = 0; k < (int)inputsOut.Length; k++)
		{
			inputsOut[k] = new InputThread.InputStruct();
		}
		_lastFrameData = new InputThread.InputStruct();
		_threadActive = true;
		_updateThread = new Thread(new ThreadStart(SuperFastLoop));
		_updateThread.Start();
		_time = Time.time;
		reset = new AutoResetEvent(false);
	}

	private void EmptyQueue()
	{
	}

	private float GetVel(float thisVal, float lastVal, long thisTime, long lastTime)
	{
		return (thisVal - lastVal) / ((float)(thisTime - lastTime) * 1E-07f);
	}

	private void InputUpdate()
	{
		prevState = state;
		state = GamePad.GetState(playerIndex);
		if (_pos < _maxLength)
		{
			if (state.IsConnected)
			{
				InputThread.InputStruct inputStruct = inputsIn[_pos];
				InputFilter inputFilter = leftXFilter;
				GamePadThumbSticks thumbSticks = state.ThumbSticks;
				GamePadThumbSticks.StickValue left = thumbSticks.Left;
				inputStruct.leftX = inputFilter.Filter((double)left.X);
				InputThread.InputStruct inputStruct1 = inputsIn[_pos];
				InputFilter inputFilter1 = leftYFilter;
				thumbSticks = state.ThumbSticks;
				left = thumbSticks.Left;
				inputStruct1.leftY = inputFilter1.Filter((double)left.Y);
				InputThread.InputStruct inputStruct2 = inputsIn[_pos];
				InputFilter inputFilter2 = rightXFilter;
				thumbSticks = state.ThumbSticks;
				left = thumbSticks.Right;
				inputStruct2.rightX = inputFilter2.Filter((double)left.X);
				InputThread.InputStruct inputStruct3 = inputsIn[_pos];
				InputFilter inputFilter3 = rightYFilter;
				thumbSticks = state.ThumbSticks;
				left = thumbSticks.Right;
				inputStruct3.rightY = inputFilter3.Filter((double)left.Y);
				inputsIn[_pos].time = DateTime.UtcNow.Ticks;
				inputsIn[_pos].leftXVel = GetVel(inputsIn[_pos].leftX, _lastFrameData.leftX, inputsIn[_pos].time, _lastFrameData.time);
				inputsIn[_pos].leftYVel = GetVel(inputsIn[_pos].leftY, _lastFrameData.leftY, inputsIn[_pos].time, _lastFrameData.time);
				inputsIn[_pos].rightXVel = GetVel(inputsIn[_pos].rightX, _lastFrameData.rightX, inputsIn[_pos].time, _lastFrameData.time);
				inputsIn[_pos].rightYVel = GetVel(inputsIn[_pos].rightY, _lastFrameData.rightY, inputsIn[_pos].time, _lastFrameData.time);
				_lastFrameData.leftX = inputsIn[_pos].leftX;
				_lastFrameData.leftY = inputsIn[_pos].leftY;
				_lastFrameData.rightX = inputsIn[_pos].rightX;
				_lastFrameData.rightY = inputsIn[_pos].rightY;
				_lastFrameData.time = inputsIn[_pos].time;
				_pos++;
				return;
			}
			inputsIn[_pos].leftX = leftXFilter.Filter((double)((Mathf.Abs(inputController.player.GetAxis("LeftStickX")) < 0.1f ? 0f : inputController.player.GetAxis("LeftStickX"))));
			inputsIn[_pos].leftY = leftYFilter.Filter((double)inputController.player.GetAxis("LeftStickY"));
			inputsIn[_pos].rightX = rightXFilter.Filter((double)((Mathf.Abs(inputController.player.GetAxis("RightStickX")) < 0.1f ? 0f : inputController.player.GetAxis("RightStickX"))));
			inputsIn[_pos].rightY = rightYFilter.Filter((double)inputController.player.GetAxis("RightStickY"));
			inputsIn[_pos].time = DateTime.UtcNow.Ticks;
			inputsIn[_pos].leftXVel = GetVel(inputsIn[_pos].leftX, _lastFrameData.leftX, inputsIn[_pos].time, _lastFrameData.time);
			inputsIn[_pos].leftYVel = GetVel(inputsIn[_pos].leftY, _lastFrameData.leftY, inputsIn[_pos].time, _lastFrameData.time);
			inputsIn[_pos].rightXVel = GetVel(inputsIn[_pos].rightX, _lastFrameData.rightX, inputsIn[_pos].time, _lastFrameData.time);
			inputsIn[_pos].rightYVel = GetVel(inputsIn[_pos].rightY, _lastFrameData.rightY, inputsIn[_pos].time, _lastFrameData.time);
			_lastFrameData.leftX = inputsIn[_pos].leftX;
			_lastFrameData.leftY = inputsIn[_pos].leftY;
			_lastFrameData.rightX = inputsIn[_pos].rightX;
			_lastFrameData.rightY = inputsIn[_pos].rightY;
			_lastFrameData.time = inputsIn[_pos].time;
			_pos++;
		}
	}

	private Vector2 MaxVelLastUpdate(bool right)
	{
		_velxMax = 0f;
		_velyMax = 0f;
		for (int i = 1; i < _lastPosClamp; i++)
		{
			_velxTemp = (right ? inputsOut[i].rightXVel : inputsOut[i].leftXVel);
			if (Mathf.Abs(_velxTemp) > Mathf.Abs(_velxMax))
			{
				_velxMax = _velxTemp;
			}
		}
		for (int j = 1; j < _lastPosClamp; j++)
		{
			_velyTemp = (right ? inputsOut[j].rightYVel : inputsOut[j].leftYVel);
			if (Mathf.Abs(_velyTemp) > Mathf.Abs(_velyMax))
			{
				_velyMax = _velyTemp;
			}
		}
		return new Vector2(_velxMax, _velyMax);
	}

	private void OnApplicationQuit()
	{
		_threadActive = false;
		_updateThread.Abort();
		_updateThread.Join();
	}

	private void OnDestroy()
	{
		_threadActive = false;
		_updateThread.Abort();
		_updateThread.Join();
	}

	private void OnDisable()
	{
		_threadActive = false;
		_updateThread.Abort();
		_updateThread.Join();
	}

	private void SuperFastLoop()
	{
		long ticks = DateTime.UtcNow.Ticks;
		count = 0;
		while (_threadActive)
		{
			if (DateTime.UtcNow.Ticks - ticks >= (long)10000000)
			{
				ThreadedUpdatesPerSecond = count;
				count = 0;
				ticks = DateTime.UtcNow.Ticks;
			}
			count++;
			InputUpdate();
			reset.WaitOne(refreshRate);
		}
	}

	public void Update()
	{
		if (!playerIndexSet || !prevState.IsConnected)
		{
			for (int i = 0; i < 4; i++)
			{
				PlayerIndex playerIndex = (PlayerIndex)i;
				if (!GamePad.GetState(playerIndex, GamePadDeadZone.None).IsConnected)
				{
					directInput = true;
				}
				else
				{
					directInput = false;
					this.playerIndex = playerIndex;
					playerIndexSet = true;
				}
			}
		}
		PlayerController.Instance.inputController.debugUI.SetThreadActive(_threadActive);
		if (Time.time - _time >= 1f)
		{
			RegularUpdatesPerSecond = _updateCount;
			_updateCount = 0;
			_time = Time.time;
		}
		_updateCount++;
		inputTimer += Time.deltaTime;
		if (inputTimer > inputInterval)
		{
			inputTimer = 0f;
			lock (_threadLock)
			{
				_lastPos = _pos;
				Array.Copy(inputsIn, inputsOut, (int)inputsIn.Length);
				_pos = 0;
			}
		}
	}

	public class InputStruct
	{
		public float leftX;

		public float leftY;

		public float rightX;

		public float rightY;

		public float leftXVel;

		public float leftYVel;

		public float rightXVel;

		public float rightYVel;

		public long time;

		public InputStruct()
		{
		}
	}
}