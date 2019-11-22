using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace WF
{
	enum MAP_SIZE
	{
		MAP_SIZE_X = 14,
		MAP_SIZE_Y = 6,
	}

	enum DIR
	{
		NONE,
		LEFT,
		RIGHT,
		UP,
		DOWN,
	}

	enum WAY_TYPE
	{
		NONE,
		WAY_1,
		WAY_2,
		WAY_3,
		WAY_4,
		WAY_5,
		WAY_6,
	}

	public partial class Form1 : Form
	{
		Point _pos = new Point(0, 250);
		Size _size = new Size(50, 50);

		PictureBox[,] _arrPicBox = new PictureBox[(int)MAP_SIZE.MAP_SIZE_Y, (int)MAP_SIZE.MAP_SIZE_X];
		WAY_TYPE[,] _arrWayType = new WAY_TYPE[(int)MAP_SIZE.MAP_SIZE_Y, (int)MAP_SIZE.MAP_SIZE_X];

		Image _image;
		WAY_TYPE _wayType;

		PictureBox _train;
		Point _trainPos = new Point(60, 265);
		Size _trainSize1 = new Size(18, 30); //18,30
		Size _trainSize2 = new Size(30, 18); //18,30
		DIR _trainDir = DIR.NONE;

		bool _isTrainHorizon = true;
		Point _traingMidPoint = new Point();

		bool _isCancleClick = false;

		System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
		System.Windows.Forms.Timer _startTimer = new System.Windows.Forms.Timer();
		bool _isSafe = true;

		int _stage = 0;
		bool _isStageClear = false;
		Random rand = new Random();

		Point _MousePos = new Point();

		public Form1()
		{
			InitializeComponent();

			for (int i = 0; i < (int)MAP_SIZE.MAP_SIZE_Y; i++)
			{
				for (int j = 0; j < (int)MAP_SIZE.MAP_SIZE_X; j++)
				{
					_arrPicBox[i, j] = new PictureBox();
					_pos.X += 50;
					_arrPicBox[i, j].Location = _pos;
					_arrPicBox[i, j].Size = _size;
					_arrPicBox[i, j].SizeMode = PictureBoxSizeMode.StretchImage;
					_arrPicBox[i, j].Click += new EventHandler(SetWay);
					_arrPicBox[i, j].BackColor = SystemColors.ActiveCaption;
					Controls.Add(_arrPicBox[i, j]);
					_arrWayType[i, j] = WAY_TYPE.NONE;
				}

				_pos.Y += 50;
				_pos.X = 0;
			}

			_train = pictureBox1;

			Init();
			_timer.Interval = 1;
			_timer.Tick += timer_Tick;

			_startTimer.Interval = 5000;
			_startTimer.Tick += StartButtonClick;
		}

		private void Init()
		{
			_image = null;

			for (int i = 0; i < (int)MAP_SIZE.MAP_SIZE_Y; i++)
			{
				for (int j = 0; j < (int)MAP_SIZE.MAP_SIZE_X; j++)
				{
					_arrPicBox[i, j].Image = null;
					_arrWayType[i, j] = WAY_TYPE.NONE;
				}
			}

			_arrPicBox[0, 0].Image = global::WF.Properties.Resources._001;
			_arrPicBox[(int)MAP_SIZE.MAP_SIZE_Y - 1, (int)MAP_SIZE.MAP_SIZE_X - 1].Image = global::WF.Properties.Resources._001;
			_arrWayType[0, 0] = WAY_TYPE.WAY_1;
			_arrWayType[(int)MAP_SIZE.MAP_SIZE_Y - 1, (int)MAP_SIZE.MAP_SIZE_X - 1] = WAY_TYPE.WAY_1;

			_isTrainHorizon = true;
			_isSafe = true;
			_train.Image = Properties.Resources.train2;
			_traingMidPoint.X = _arrPicBox[0, 0].Location.X + 25;
			_traingMidPoint.Y = _arrPicBox[0, 0].Location.Y + 25;
			_train.Size = _trainSize2;
			SetTrainPos();
			_trainDir = DIR.RIGHT;

			SetBlock();

			_isStageClear = false;
			_timer.Stop();
			_startTimer.Start();
		}

		private void SetBlock()
		{
			int temp = 0;
			while (temp < _stage)
			{
				int i = rand.Next(0, (int)MAP_SIZE.MAP_SIZE_Y);
				int j = rand.Next(0, (int)MAP_SIZE.MAP_SIZE_X);
				if (_arrPicBox[i, j].Image == null)
				{
					_arrPicBox[i, j].Image = Properties.Resources.Block;
					temp++;
				}
			}
		}

		// 길 버튼 
		private void button1_Click(object sender, EventArgs e)
		{
			_image = Properties.Resources._001;
			_wayType = WAY_TYPE.WAY_1;
			_isCancleClick = false;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			_image = Properties.Resources._002;
			_wayType = WAY_TYPE.WAY_2;
			_isCancleClick = false;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			_image = Properties.Resources._003;
			_wayType = WAY_TYPE.WAY_3;
			_isCancleClick = false;
		}

		private void button4_Click(object sender, EventArgs e)
		{
			_image = Properties.Resources._004;
			_wayType = WAY_TYPE.WAY_4;
			_isCancleClick = false;
		}

		private void button5_Click(object sender, EventArgs e)
		{
			_image = Properties.Resources._005;
			_wayType = WAY_TYPE.WAY_5;
			_isCancleClick = false;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			_image = Properties.Resources._006;
			_wayType = WAY_TYPE.WAY_6;
			_isCancleClick = false;
		}

		// 픽쳐박스에 길 이미지 삽입하기
		private void SetWay(object sender, EventArgs e)
		{
			_MousePos.X = MousePosition.X;
			_MousePos.Y = MousePosition.Y;
			int j = (PointToClient(_MousePos).X - 50) / 50;
			int i = (PointToClient(_MousePos).Y - 250) / 50;
			//int j = (PointToClient(new Point(MousePosition.X, MousePosition.Y)).X - 50)/ 50;
			//int i = (PointToClient(new Point(MousePosition.X, MousePosition.Y)).Y - 250) / 50;

			// 예외처리
			if (i >= (int)MAP_SIZE.MAP_SIZE_Y || j >= (int)MAP_SIZE.MAP_SIZE_X) return;

			if ((i == 0 && j == 0) || (i == (int)MAP_SIZE.MAP_SIZE_Y - 1 && j == (int)MAP_SIZE.MAP_SIZE_X - 1)) return;

			if (_arrPicBox[i, j].Image != null && _isCancleClick == true)
			{
				_arrPicBox[i, j].Image = null;
				_arrWayType[i, j] = WAY_TYPE.NONE;
			}

			if (_arrPicBox[i, j].Image == null)
			{
				_arrPicBox[i, j].Image = _image;
				_arrWayType[i, j] = _wayType;
			}
		}

		// 망치 버튼 클릭
		private void FixButtonClick(object sender, EventArgs e)
		{
			_isCancleClick = true;
			_image = null;
			_wayType = WAY_TYPE.NONE;
		}

		// 시작 버튼 클릭
		private void StartButtonClick(object sender, EventArgs e)
		{
			if (_isSafe == false) return;
			_timer.Start();
			_startTimer.Stop();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			// timer 마다 이동
			Move(_trainDir);

			// 타일들 75, 275 -> 125,  325-> 125, 125 50씩 증가
			// 중앙 값이 타일 중앙으로 오면 방향 서치
			// 방향 서치 후 이미지 변환
			int j = (_traingMidPoint.X - 50) / 50;
			int i = (_traingMidPoint.Y - 250) / 50;

			if (i < 0 || i > (int)MAP_SIZE.MAP_SIZE_Y - 1 || j < 0 || j > (int)MAP_SIZE.MAP_SIZE_X - 1)
			{
				textBox1.Text = "열차 사고";
				_isSafe = false;
				_timer.Stop();
				return;
			}

			Point tempP = _arrPicBox[i, j].Location;
			tempP.X += 25;
			tempP.Y += 25;

			if (_traingMidPoint == tempP)
			{
				SearchDir(_arrWayType[i, j]);
			}

			// 중앙 값이 타일 경계 값일때 다음 타일 검사
			if (_traingMidPoint.X % 50 == 0 || _traingMidPoint.Y % 50 == 0)
			{
				if (_trainDir == DIR.LEFT)
				{
					if (j - 1 < 0)
					{
						textBox1.Text = "열차 사고";
						_isSafe = false;
						_timer.Stop();
						return;
					}
					CheckNextWay(_arrWayType[i, j - 1]);
				}
				else if (_trainDir == DIR.UP)
				{
					if (i - 1 < 0)
					{
						textBox1.Text = "열차 사고";
						_isSafe = false;
						_timer.Stop();
						return;
					}
					CheckNextWay(_arrWayType[i - 1, j]);
				}
				else CheckNextWay(_arrWayType[i, j]);
			}

			// 마지막 부분인가
			tempP = _arrPicBox[(int)MAP_SIZE.MAP_SIZE_Y - 1, (int)MAP_SIZE.MAP_SIZE_X - 1].Location;
			tempP.X += 25;
			tempP.Y += 25;
			if (_traingMidPoint == tempP)
			{
				textBox1.Text = "열차 도착";
				_isStageClear = true;
				_timer.Stop();
			}

			// 기차가 안전한가
			if (_isSafe == false)
			{
				textBox1.Text = "열차 사고";
				_timer.Stop();
			}
		}

		private void Move(DIR dir)
		{
			switch (dir)
			{
				case DIR.NONE:
					_isSafe = false;
					break;
				case DIR.LEFT:
					_traingMidPoint.X -= 1;
					break;
				case DIR.RIGHT:
					_traingMidPoint.X += 1;
					break;
				case DIR.UP:
					_traingMidPoint.Y -= 1;
					break;
				case DIR.DOWN:
					_traingMidPoint.Y += 1;
					break;
				default:
					break;
			}

			SetTrainPos();
		}

		private void SearchDir(WAY_TYPE waytype)
		{
			switch (waytype)
			{
				case WAY_TYPE.WAY_1:
				case WAY_TYPE.WAY_2:
					return;
				case WAY_TYPE.WAY_3:
					{
						if (_trainDir == DIR.UP) { _trainDir = DIR.RIGHT; }
						else if (_trainDir == DIR.LEFT) { _trainDir = DIR.DOWN; }
					}
					break;
				case WAY_TYPE.WAY_4:
					{
						if (_trainDir == DIR.RIGHT) { _trainDir = DIR.DOWN; }
						else if (_trainDir == DIR.UP) { _trainDir = DIR.LEFT; }
					}
					break;
				case WAY_TYPE.WAY_5:
					{
						if (_trainDir == DIR.DOWN) { _trainDir = DIR.RIGHT; }
						else if (_trainDir == DIR.LEFT) { _trainDir = DIR.UP; }
					}
					break;
				case WAY_TYPE.WAY_6:
					{
						if (_trainDir == DIR.RIGHT) { _trainDir = DIR.UP; }
						else if (_trainDir == DIR.DOWN) { _trainDir = DIR.LEFT; }
					}
					break;
				default:
					break;
			}

			_isTrainHorizon = !_isTrainHorizon;
			ChangeTrainImage();

		}

		private void ChangeTrainImage()
		{
			if (_isTrainHorizon)// 가로
			{
				_train.Image = Properties.Resources.train2;
				_train.Size = _trainSize2;
			}
			else // 세로
			{
				_train.Image = Properties.Resources.train;
				_train.Size = _trainSize1;
			}

			SetTrainPos();
		}

		private void SetTrainMidPos()
		{
			if (_isTrainHorizon)
			{
				_traingMidPoint.X = _train.Location.X + 9;
				_traingMidPoint.Y = _train.Location.Y + 15;
			}
			else
			{
				_traingMidPoint.X = _train.Location.X + 15;
				_traingMidPoint.Y = _train.Location.Y + 9;
			}
		}

		private void SetTrainPos()
		{
			Point temp = _traingMidPoint;
			if (_isTrainHorizon)
			{
				temp.X -= 15;
				temp.Y -= 9;
				_train.Location = temp;
			}
			else
			{
				temp.X -= 9;
				temp.Y -= 15;
				_train.Location = temp;
			}
		}

		private void CheckNextWay(WAY_TYPE waytype)
		{
			switch (waytype)
			{
				case WAY_TYPE.NONE:
					_isSafe = false;
					break;
				case WAY_TYPE.WAY_1:
					{
						if (_trainDir != DIR.LEFT && _trainDir != DIR.RIGHT) { _isSafe = false; }
					}
					break;
				case WAY_TYPE.WAY_2:
					{
						if (_trainDir != DIR.UP && _trainDir != DIR.DOWN) { _isSafe = false; }
					}
					break;
				case WAY_TYPE.WAY_3:
					{
						if (_trainDir != DIR.UP && _trainDir != DIR.LEFT) { _isSafe = false; }
					}
					break;
				case WAY_TYPE.WAY_4:
					{
						if (_trainDir != DIR.UP && _trainDir != DIR.RIGHT) { _isSafe = false; }
					}
					break;
				case WAY_TYPE.WAY_5:
					{
						if (_trainDir != DIR.LEFT && _trainDir != DIR.DOWN) { _isSafe = false; }
					}
					break;
				case WAY_TYPE.WAY_6:
					{
						if (_trainDir != DIR.RIGHT && _trainDir != DIR.DOWN) { _isSafe = false; }
					}
					break;
				default:
					break;
			}
		}

		private void button9_Click(object sender, EventArgs e)
		{
			if (_isStageClear == false) return;
			_stage += 2;
			Init();
			textBox1.Text = "스테이지 : " + ((_stage / 2) + 1).ToString() + " 시작";
		}

		private void button10_Click(object sender, EventArgs e)
		{
			_stage = 0;
			Init();
			textBox1.Text = "스테이지 : 1 시작";
		}

		private void button11_Click(object sender, EventArgs e)
		{
			Init();
			if (_stage == 0)
			{
				textBox1.Text = "스테이지 : 1 시작";
			}
			else
			{
				textBox1.Text = "스테이지 : " + (_stage / 2).ToString() + " 시작";
			}
		}
	}
}
