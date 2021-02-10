using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;

namespace BeadandoWPF
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();
        List<Key> _pressedKeys = new List<Key>();
        Random rnd = new Random();

        bool moveLeft, moveRight;
        int enemySpriteCounter = 0;
        int enemyCounter = 100;
        int playerSpeed = 15;
        int limit = 50;
        int score = 0;
        int health = 100;
        int enemySpeed = 10;

        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(25);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            Game.Focus();
            
            ImageBrush bg = new ImageBrush();
            
            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,/Images/background.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0, 0, 0.15, 0.15);
            bg.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            Game.Background = bg;

            ImageBrush playerImg = new ImageBrush();
            playerImg.ImageSource = new BitmapImage(new Uri("pack://application:,,/Images/player.png"));
            Player.Fill = playerImg;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);
            enemyCounter -= 1;

            scoreText.Content = "Score: " + score;
            healthText.Content = "Health: " + health;

            if (enemyCounter < 0)
            {
                MakeEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(Player) - 15 > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - playerSpeed);
            }

            if (moveRight == true && Canvas.GetLeft(Player) + 40 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + playerSpeed);
            }

            foreach (var item in Game.Children.OfType<Rectangle>())
            {
                if (item is Rectangle && (string)item.Tag == "projectile")
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) - 20);

                    Rect projectileHitBox = new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.Width, item.Height);
                    
                    if (Canvas.GetTop(item) < 10)
                    {
                        itemRemover.Add(item);
                    }

                    foreach (var i in Game.Children.OfType<Rectangle>())
                    {
                        if (i is Rectangle && (string)i.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(i), Canvas.GetTop(i), i.Width, i.Height);

                            if (projectileHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(i);
                                itemRemover.Add(item);
                                score++;
                            }
                        }
                    }
                }
                if (item is Rectangle && (string)item.Tag == "enemy")
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) + enemySpeed);

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.Width, item.Height);

                    if (Canvas.GetTop(item) > 750)
                    {
                        itemRemover.Add(item);
                        health -= 10;
                    }

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(item);
                        health -= 5;
                        score++;
                    }
                }
            }
            foreach (Rectangle item in itemRemover)
            {
                Game.Children.Remove(item);
            }
            if (score >= 10)
            {
                limit = 44;
                enemySpeed = 11;
            }
            if (score >= 50)
            {
                limit = 33;
                enemySpeed = 13;
            }
            if (score >= 100)
            {
                limit = 22;
                enemySpeed = 16;
            }
            if (score >= 200)
            {
                limit = 11;
                enemySpeed = 18;
            }
            if (health == 0)
            {
                gameTimer.Stop();
                healthText.Content = "Health: 0";
                healthText.Foreground = Brushes.Red;
                
                MessageBox.Show("You have gained a score of:"+ score + Environment.NewLine + "Press Ok to play again.", "Game Over");
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            foreach (Key key in _pressedKeys)
            {
                if (key == Key.Left)
                {
                    moveLeft = true;
                }
                if (key == Key.Right)
                {
                    moveRight = true;
                }
                if (key == Key.P)
                {
                    gameTimer.Stop();
                }
                if (key == Key.R)
                {
                    gameTimer.Start();
                }
                if (key == Key.Escape)
                {
                    Close();
                }
                if (key == Key.Space)
                {
                    Rectangle newProjectile = new Rectangle
                    {
                        Tag = "projectile",
                        Height = 10,
                        Width = 3,
                        Fill = Brushes.White,
                        Stroke = Brushes.Green,
                    };
                    Canvas.SetLeft(newProjectile, Canvas.GetLeft(Player) + Player.Width / 2);
                    Canvas.SetTop(newProjectile, Canvas.GetTop(Player) + newProjectile.Height);

                    Game.Children.Add(newProjectile);
                }
            }
        }
        public void OnKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _pressedKeys.Add(e.Key);
            }
            if (e.Key == Key.Right)
            {
                _pressedKeys.Add(e.Key);
            }
            if (e.Key == Key.P)
            {
                _pressedKeys.Add(e.Key);
            }
            if (e.Key == Key.R)
            {
                _pressedKeys.Add(e.Key);
            }
            if (e.Key == Key.Escape)
            {
                _pressedKeys.Add(e.Key);
            }
            if (e.Key == Key.Space)
            {
                _pressedKeys.Add(e.Key);
            }
        }
        public void OnKeyUp(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _pressedKeys.RemoveAll(item => item == e.Key);
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                _pressedKeys.RemoveAll(item => item == e.Key);
                moveRight = false;
            }
            if (e.Key == Key.P)
            {
                _pressedKeys.RemoveAll(item => item == e.Key);
            }
            if (e.Key == Key.R)
            {
                _pressedKeys.RemoveAll(item => item == e.Key);
            }
            if (e.Key == Key.Escape)
            {
                _pressedKeys.RemoveAll(item => item == e.Key);
            }
            if (e.Key == Key.Space)
            {
                _pressedKeys.RemoveAll(item => item == e.Key);
            }
        }
        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();
            enemySpriteCounter = rnd.Next(1, 3);
            switch (enemySpriteCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,/Images/enemy.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,/Images/Asteroid1.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,/Images/Asteroid2.png"));
                    break;
            }
            Rectangle newEnemy = new Rectangle
            {
                Tag = "enemy",
                Height = 24,
                Width = 24,
                Fill = enemySprite,
            };
            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rnd.Next(30, 430));
            Game.Children.Add(newEnemy);
        }
    }
}
