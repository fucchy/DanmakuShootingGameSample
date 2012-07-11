﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;

namespace TohoSTG
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Bitmap haikei;          // 背景宇宙
        Graphics g;
        List<Bullet> bullets;  // 弾幕プールの1グループ
        //Bullet b1;      // とりあえずなんだからね！
        //Bullet b2;      // とりあえずなんだからね！
        Random r;
        Jiki j1;
        WindowsMediaPlayer wmp;

        private const int width = 479;
        private const int height = 303;
        private Boolean isAlive;
        //private Dictionary<Keys, bool> isPressed;
        private PadState padState;
        private List<Enemy> enemies;
        const int INTERVAL = 33;    // (INTERVAL + a) * 30 = 1000あたりを狙う
        private int score;
        private int RotationPhase;
        //private DateTime t0;
        private List<TimeSpan> stagetime;
        private List<DateTime> startedTime;

        private void reset()
        {
            j1 = new Jiki(240, 260, width, height);    // 初期位置はてきとー
            isAlive = true;

            score = 0;
            RotationPhase = 0;

            padState = new PadState();
            //isPressed = new Dictionary<Keys, bool>();
            //isPressed.Add(Keys.A, false);
            //isPressed.Add(Keys.D, false);
            //isPressed.Add(Keys.W, false);
            //isPressed.Add(Keys.S, false);

            bullets = new List<Bullet>();   // 弾幕プールの1グループ
            enemies = new List<Enemy>();    // 敵の一味
            timer1.Interval = INTERVAL;   // ミリ秒
            stagetime = new List<TimeSpan>();
            startedTime = new List<DateTime>();
            startedTime.Add(DateTime.Now);
            //startedTime[0] = DateTime.Now;    // まだないなら変更できない
            //t0 = DateTime.Now;

            timer1.Start();
        }

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            bmp = new Bitmap(width, height);
            haikei = new Bitmap(@"C:\Users\s\Documents\Visual Studio 2010\Projects\TohoSTG\TohoSTG\image\clippedGradiuss3_479x303.png");
            g = Graphics.FromImage(bmp);
            r = new Random();
            pictureBox1.Image = bmp;

            wmp = new WindowsMediaPlayer();

            wmp.URL = @"D:\Download\ByFirefox\gamebgmsozai\sht_a02.mp3";
            wmp.settings.volume = 5;
            wmp.settings.playCount = 0;
            wmp.settings.setMode("loop", true);
            wmp.controls.play();

            reset();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    int num = 200;
        //    double constant = 1.5;
        //    for (int i = 0; i < num; i++)
        //    {
        //        //Bullet b = new Bullet(150, 100, Math.Pow(-1.0, (double)i), 2.7);
        //        Bullet b = new Bullet(150, 100, Math.Cos(Math.PI * 2 * i / num) * constant, - Math.Sin(Math.PI * 2 * i / num) * constant);
        //        bullets.Add(b);
        //        b.draw(this, g);
        //    }
        //    //b1 = new Bullet(150, 100, 2.2, 3.7);
        //    //b1.draw(this, g);
        //    //b2 = new Bullet(150, 100, -3, -2);
        //    //b2.draw(this, g);

        //    //timer1.Interval = 25;   // ミリ秒
        //    //timer1.Start();

        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (padState.押された(PadState.Buttons.reset)) reset();
            if (isAlive == false)
            {
                timer1.Start();
                return;
            }

            // 弾幕の生成
            //if (r.Next(100) == 0)
            //Func<Enemy> a = ((Enemy x) => x.IsAlive && x.Y > height * 0.8);
            var e2 = enemies.FindAll(x => x.IsAlive && x.Y < height * 0.6);
            if (r.Next(20) == 0 && e2.Count > 0)   // 敵がいないとRandomが0を返すので
            {
                //int index = r.Next(0);  // 弾幕を出す敵の抽選
                bool isAliveEnemy = false;
                Enemy enemy;
                do
                {
                    int index = r.Next(e2.Count);  // 弾幕を出す敵の抽選
                    enemy = e2[index];
                    isAliveEnemy = enemy.IsAlive;
                } while (isAliveEnemy == false);
                int x = (int)enemy.X;
                int y = (int)enemy.Y;

                //int x = width / 4 + r.Next(2 * width / 4);
                //int y = height/ 4 + r.Next(2 * height/ 4);

                //int num = 17;
                //int num = 37;
                int num = 29;
                //double constant = 1.5;  // 弾丸の速度
                
                //double constant = 6;  // 弾丸の速度
                double constant = 4 + r.Next(4);  // 弾丸の速度、ばらつきを持たせてみた
                
                for (int i = 0; i < num; i++)
                {
                    //Bullet b = new Bullet(150, 100, Math.Pow(-1.0, (double)i), 2.7);
                    //Bullet b = new Bullet(x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant);
                    Bullet b = new Bullet(Bullet.Sides.teki, x, y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant);
                    bullets.Add(b);
                    b.draw(this, g);
                }
            }
            if (r.Next(20) == 0)
            // 敵の誕生
            {
                int ix = r.Next(width + 32) - 24;
                Enemy enemy = new Enemy(ix, -16, 0, 8);    // 引数は出現位置
                enemies.Add(enemy);
            }

            //g.Clear(Color.White);
            //g.Clear(Color.Black);
            
            // TODO: 背景画像でテストプレイしにくいので要検討
            //g.DrawImage(haikei, 0, 0);
            g.Clear(Color.Black);

            //j1.move(isPressed);

            if (padState.押された(PadState.Buttons.button1))
            {
                int myBulletSpeed = -8;
                Bullet b = new Bullet(Bullet.Sides.mikata, j1.X + j1.Width / 2, j1.Y, 0, myBulletSpeed);
                bullets.Add(b);
                padState.clearPressed(PadState.Buttons.button1);
            }
            j1.move(padState);
            j1.draw(g);

            // 敵味方問わず弾丸の移動と描画と当たり判定
            foreach (var bullet in bullets)
            {
                //item.zanzou(this, g);
                //bullet.move(new Point(width / 2, height / 2));
                bullet.move();
                bullet.draw(this, g);
                switch (bullet.Side)
                {
                    case Bullet.Sides.teki:
                        if (bullet.inquire(j1))
                        {
                            //MessageBox.Show("死亡。");
                            isAlive = false;    // 死亡
                            //timer1.Enabled = false;
                            //timer1.Stop();
                            break;
                            //Close();
                        }
                        break;
                    case Bullet.Sides.mikata:
                        foreach (var enemy in enemies)
                        {
                            if (bullet.inquire(enemy))
                            {
                                enemy.die();
                                score += 100;
                                //stagetime[1 + score / 2000] = DateTime.Now - t0;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            bullets.RemoveAll(x => x.isFadeOut(width, height));

            foreach (var enemy in enemies)
            {
                //width / 2, height / 2
                int gy = (int)(height / 2 + /*keisu*/40 * Math.Cos(2 * Math.PI * RotationPhase / 60));
                int gx = (int)(width  / 2 + /*keisu*/100 * Math.Sin(-2 * Math.PI * RotationPhase / 60));

                enemy.move(new Point(gx, gy));
                enemy.draw(g);
            }
            enemies.RemoveAll(x => x.isFadeOut(width, height));

            g.DrawString(score.ToString(), DefaultFont, Brushes.White, 16, height - 32);

            int localStageConstant = 2000;
            if (startedTime.Count > 1 + score / localStageConstant)
            {
                startedTime[1 + score / localStageConstant] = DateTime.Now;
            }
            else
            {
                startedTime.Add(DateTime.Now);
            }
            for (int i = 1; i < startedTime.Count; i++)
			{
                TimeSpan thisstagetime = (startedTime[i] - startedTime[i - 1]);
                //TimeSpan thisstagetime = (startedTime[i] - startedTime[i - 1]) - new TimeSpan((startedTime[i] - startedTime[i - 1]).Milliseconds);
                //string timestring = (startedTime[i] - startedTime[i - 1]).ToString();
                //g.DrawString(String.Format("Stage {0}: {1}", i, timestring), DefaultFont, Brushes.White, width - 160, 16 + i * 16);
                g.DrawString(String.Format("Stage {0}: {1}", i, new TimeSpan(thisstagetime.Hours, thisstagetime.Minutes, thisstagetime.Seconds)), DefaultFont, Brushes.White, width - 160, 16 + i * 16);
                //g.DrawString((startedTime[i] - startedTime[i - 1]).ToString(), DefaultFont, Brushes.White, width - 160, 16 + i * 16);
			}
            //foreach (var item in startedTime)
            //{
            //    g.DrawString(String.Format("Stage {0}: {1}", startedTime.IndexOf(item), item),                
            //}
            ////if (score >= 2000)
            //if (score >= 200)
            //{

            //    g.DrawString("Stage 1: " + t.ToString(), DefaultFont, Brushes.White, width - 160, 64);

            //    g.DrawString("Stage 1: " + t.ToString(), DefaultFont, Brushes.White, width - 160, 64);
            //}

            //b1.zanzou(this, g);
            //b1.move();
            //b1.draw(this, g);
            //b2.zanzou(this, g);
            //b2.move();
            //b2.draw(this, g);

            Refresh();

            //textBox1.AppendText("Tick.\n");
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Left = 900;
            //this.Top = 300;
            //Focus();
            button1.Focus();
            KeyPreview = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Console.WriteLine(e.KeyCode.ToString());
            padState.KeyDown(e);
            //if (e.KeyCode == Keys.A) isPressed[Keys.A] = true;
            //if (e.KeyCode == Keys.D) isPressed[Keys.D] = true;
            //if (e.KeyCode == Keys.W) isPressed[Keys.W] = true;
            //if (e.KeyCode == Keys.S) isPressed[Keys.S] = true;
            

            //if (e.KeyCode == Keys.A) j1.difference(-1, 0);
            //if (e.KeyCode == Keys.D) j1.difference(+1, 0);
            //if (e.KeyCode == Keys.W) j1.difference(0, -1);
            //if (e.KeyCode == Keys.S) j1.difference(0, +1);
            //if (e.KeyCode == Keys.A)
            //{
            //    Console.WriteLine("A Pressed.");
            //}
            e.Handled = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            padState.KeyUp(e);
            //if (e.KeyCode == Keys.A) isPressed[Keys.A] = false;
            //if (e.KeyCode == Keys.D) isPressed[Keys.D] = false;
            //if (e.KeyCode == Keys.W) isPressed[Keys.W] = false;
            //if (e.KeyCode == Keys.S) isPressed[Keys.S] = false;

            e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reset();
        }

        //private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        //{
        //    int num = 37;
        //    double constant = 1.5;
        //    for (int i = 0; i < num; i++)
        //    {
        //        //Bullet b = new Bullet(150, 100, Math.Pow(-1.0, (double)i), 2.7);
        //        Bullet b = new Bullet(e.X, e.Y, Math.Cos(Math.PI * 2 * i / num) * constant, -Math.Sin(Math.PI * 2 * i / num) * constant);
        //        bullets.Add(b);
        //        b.draw(this, g);
        //    }
        //}
    }
}