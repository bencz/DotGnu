/*
 * snake.cs - Sample program for the extended console routines.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Threading;

#if CONFIG_EXTENDED_CONSOLE

public class Snake
{
	// Internal state.
	private bool monochrome;
	private int originX, originY;
	private int left, top, width, height;
	private Occupied[] occupied;
	private int headX, headY;
	private int tailX, tailY;
	private Direction direction;
	private Timer snakeTimer;
	private Random random;
	private int score;
	private bool gameOver;

	// Values that may appear in the "occupied" array.
	private enum Occupied : byte
	{
		Empty,
		Star,
		Wall,
		Piece_NextLeft,
		Piece_NextRight,
		Piece_NextUp,
		Piece_NextDown

	}; // enum Occupied

	// Direction the snake is running in.
	private enum Direction
	{
		Left,
		Right,
		Up,
		Down

	}; // enum Direction

	// Constructor.
	public Snake(bool mono)
			{
				originX = Console.CursorLeft;
				originY = Console.CursorTop;
				monochrome = mono;
				random = new Random();
				NewGame();
			}

	// Set the current text attributes.
	private static void SetTextAttribute(ConsoleColor foreground,
								         ConsoleColor background)
			{
				Console.ForegroundColor = foreground;
				Console.BackgroundColor = background;
			}

	// Set the cursor position, using window-relative co-ordinates.
	private void SetCursorPosition(int x, int y)
			{
				Console.SetCursorPosition(originX + x, originY + y);
			}

	// Main entry point.
	public static void Main(String[] args)
			{
				// Initialize the console routines and put us into the
				// "alternative" screen mode under Unix.
				Console.Clear();

				// Turn off the cursor, if possible.
				Console.CursorVisible = false;

				// Set the terminal window's title.
				Console.Title = "DotGNU Snake!";

				// Create the game object.
				Snake snake;
				if(args.Length > 0 && args[0] == "--monochrome")
				{
					snake = new Snake(true);
				}
				else
				{
					snake = new Snake(false);
				}

				// Draw the initial board layout.
				snake.DrawBoard();

				// Install the timeouts that we need.
				snake.SetupTimers();

				// Enter the main key processing loop.
				snake.MainLoop();

				// Shut down the timers.
				snake.ShutdownTimers();

				// Clear the screen to the default attributes before exiting.
				Console.CursorVisible = true;
				SetTextAttribute(ConsoleColor.Gray, ConsoleColor.Black);
				Console.Clear();
			}

	// Start a new game.
	private void NewGame()
			{
				// Get the position and size of the playing area.
				left = 1;
				top = 1;
				width = Console.WindowWidth - 2;
				height = Console.WindowHeight - 3;

				// Create the "occupied" array.
				occupied = new Occupied [width * height];

				// Place the initial location of the snake.
				int length = (width / 4) * 3;
				tailX = (width - length) / 2;
				tailY = height / 2;
				headX = tailX + length - 1;
				headY = tailY;
				for(int temp = tailX; temp <= headX; ++temp)
				{
					occupied[temp + tailY * width] = Occupied.Piece_NextRight;
				}

				// Place a star in a random location.
				PlaceStar(false);

				// The initial direction is "right".
				direction = Direction.Right;

				// The score is currently nothing.
				score = 0;

				// We are currently playing a game.
				gameOver = false;
			}

	// Draw the board layout.
	private void DrawBoard()
			{
				int temp;

				// Clear the screen to the default attributes.
				SetTextAttribute(ConsoleColor.Gray, ConsoleColor.Black);
				Console.Clear();

				// Set the text attributes so that we get a solid rectangle
				// on color terminals, or regular line drawing characters
				// on monochrome terminals.
				if(!monochrome)
				{
					SetTextAttribute
						(ConsoleColor.DarkRed, ConsoleColor.DarkRed);
				}

				// Draw the top line.
				SetCursorPosition(left - 1, top - 1);
				Console.Write('+');
				for(temp = 0; temp < width; ++temp)
				{
					Console.Write('-');
				}
				Console.Write('+');

				// Draw the bottom line.
				SetCursorPosition(left - 1, top + height);
				Console.Write('+');
				for(temp = 0; temp < width; ++temp)
				{
					Console.Write('-');
				}
				Console.Write('+');

				// Draw the left and right sides.
				for(temp = 0; temp < height; ++temp)
				{
					SetCursorPosition(left - 1, top + temp);
					Console.Write('|');
					SetCursorPosition(left + width, top + temp);
					Console.Write('|');
				}

				// Draw all of the pieces on the board as it currently stands.
				int x, y;
				for(y = 0; y < height; ++y)
				{
					for(x = 0; x < width; ++x)
					{
						Occupied occ = occupied[x + y * width];
						if(occ == Occupied.Star)
						{
							SetCursorPosition(left + x, top + y);
							if(!monochrome)
							{
								SetTextAttribute
									(ConsoleColor.DarkYellow,
									 ConsoleColor.Black);
							}
							Console.Write('*');
						}
						else if(occ == Occupied.Wall)
						{
							SetCursorPosition(left + x, top + y);
							if(!monochrome)
							{
								SetTextAttribute
									(ConsoleColor.DarkRed,
									 ConsoleColor.DarkRed);
							}
							Console.Write('+');
						}
						else if(occ != Occupied.Empty)
						{
							SetCursorPosition(left + x, top + y);
							if(!monochrome)
							{
								SetTextAttribute
									(ConsoleColor.DarkGreen,
									 ConsoleColor.DarkGreen);
							}
							Console.Write('#');
						}
					}
				}

				// Print the current score.
				PrintScore();

				// Shift the cursor out of the way.
				ShiftCursor();
			}

	// Shift the cursor to an out of the way location so that it
	// doesn't clutter up the main viewing area.
	private void ShiftCursor()
			{
				SetCursorPosition(0, top + height + 1);
			}

	// Place a star on the board in a random location.
	private void PlaceStar(bool draw)
			{
				for(;;)
				{
					int x = random.Next(width);
					int y = random.Next(height);
					if(occupied[x + y * width] == Occupied.Empty)
					{
						occupied[x + y * width] = Occupied.Star;
						if(draw)
						{
							SetCursorPosition(left + x, top + y);
							if(!monochrome)
							{
								SetTextAttribute
									(ConsoleColor.DarkYellow,
									 ConsoleColor.Black);
							}
							Console.Write('*');
						}
						break;
					}
				}
			}

	// Print the current score.
	private void PrintScore()
			{
				SetCursorPosition(width - 14, height + 2);
				if(!monochrome)
				{
					SetTextAttribute
						(ConsoleColor.Gray, ConsoleColor.Black);
				}
				Console.Write("Score: {0}   ", score);
			}

	// Draw a message box, centered on-screen.
	private void MessageBox(String msg)
			{
				int temp, temp2;
				int left, top, width, height;

				// Determine the size and position of the dialog.
				width = msg.Length + 8;
				height = 5;
				left = (this.width - width) / 2 + this.left;
				top = (this.height - height) / 2 + this.top;

				// Set the color information for the border.
				if(!monochrome)
				{
					SetTextAttribute
						(ConsoleColor.DarkBlue, ConsoleColor.DarkBlue);
				}

				// Draw the top line.
				SetCursorPosition(left, top);
				Console.Write('+');
				for(temp = 2; temp < width; ++temp)
				{
					Console.Write('-');
				}
				Console.Write('+');

				// Draw the bottom line.
				SetCursorPosition(left, top + height - 1);
				Console.Write('+');
				for(temp = 2; temp < width; ++temp)
				{
					Console.Write('-');
				}
				Console.Write('+');

				// Draw the left and right sides and clear the insides.
				for(temp = 1; temp < (height - 1); ++temp)
				{
					SetCursorPosition(left, top + temp);
					Console.Write('|');
					if(!monochrome)
					{
						SetTextAttribute
							(ConsoleColor.Gray, ConsoleColor.Black);
					}
					for(temp2 = 2; temp2 < width; ++temp2)
					{
						Console.Write(' ');
					}
					if(!monochrome)
					{
						SetTextAttribute
							(ConsoleColor.DarkBlue, ConsoleColor.DarkBlue);
					}
					Console.Write('|');
				}

				// Draw the text in the dialog box.
				if(!monochrome)
				{
					SetTextAttribute
						(ConsoleColor.Gray, ConsoleColor.Black);
				}
				SetCursorPosition(left + 4, top + 2);
				Console.Write(msg);

				// Shift the cursor out of the way.
				ShiftCursor();
			}

	// Deal with the "game over" condition.
	private void GameOver()
			{
				// Mark the game as being over.
				gameOver = true;

				// Display the game over message box.
				MessageBox(String.Format
					("Game Over!  Final score was {0}!", score));
			}

	// Callback for the snake timer.
	private void SnakeTimer(Object state)
			{
				// Bail out if the game is already over.
				if(gameOver)
				{
					return;
				}

				// Determine the new position of the head, and the "piece"
				// to place at the old position of the head.
				int newX = headX;
				int newY = headY;
				Occupied piece = Occupied.Piece_NextLeft;
				switch(direction)
				{
					case Direction.Left:
					{
						--newX;
						piece = Occupied.Piece_NextLeft;
					}
					break;

					case Direction.Right:
					{
						++newX;
						piece = Occupied.Piece_NextRight;
					}
					break;

					case Direction.Up:
					{
						--newY;
						piece = Occupied.Piece_NextUp;
					}
					break;

					case Direction.Down:
					{
						++newY;
						piece = Occupied.Piece_NextDown;
					}
					break;
				}

				// Determine the new tail location.
				int newTailX = tailX;
				int newTailY = tailY;
				switch(occupied[tailX + tailY * width])
				{
					case Occupied.Piece_NextLeft:
					{
						--newTailX;
					}
					break;

					case Occupied.Piece_NextRight:
					{
						++newTailX;
					}
					break;

					case Occupied.Piece_NextUp:
					{
						--newTailY;
					}
					break;

					case Occupied.Piece_NextDown:
					{
						++newTailY;
					}
					break;
				}

				// Determine if there is a blockage at the new head position.
				bool needStar = false;
				if(newX < 0 || newX >= width ||
				   newY < 0 || newY >= height)
				{
					// We've run into an outer wall.
					GameOver();
					return;
				}
				else if(occupied[newX + newY * width] == Occupied.Star)
				{
					// We've just collected a star: make us longer.
					newTailX = tailX;
					newTailY = tailY;
					needStar = true;
					++score;
				}
				else if(occupied[newX + newY * width] != Occupied.Empty)
				{
					// We've run into ourselves or an inner wall.
					GameOver();
					return;
				}

				// Draw a new block at the head of the snake.
				if(!monochrome)
				{
					SetTextAttribute
						(ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
				}
				SetCursorPosition(newX + left, newY + top);
				Console.Write('#');
				occupied[headX + headY * width] = piece;
				occupied[newX + newY * width] = piece;
				headX = newX;
				headY = newY;

				// Erase the block at the tail of the snake.
				if(newTailX != tailX || newTailY != tailY)
				{
					if(!monochrome)
					{
						SetTextAttribute
							(ConsoleColor.Gray, ConsoleColor.Black);
					}
					SetCursorPosition(tailX + left, tailY + top);
					Console.Write(' ');
					occupied[tailX + tailY * width] = Occupied.Empty;
					tailX = newTailX;
					tailY = newTailY;
				}

				// Place a new star if we just ate one.
				if(needStar)
				{
					PlaceStar(true);
					PrintScore();
				}

				// Shift the cursor to get it out of the way.
				ShiftCursor();
			}

	// Set up the timers that we need to process snake movements
	// and the placement of stars to be eaten by the snake.
	private void SetupTimers()
			{
				if(snakeTimer == null)
				{
					snakeTimer = new Timer
						(new TimerCallback(SnakeTimer), null, 200, 200);
				}
				else
				{
					snakeTimer.Change(200, 200);
				}
			}

	// Shut down the timers.
	private void ShutdownTimers()
			{
				// Dispose of the timer threads.
			#if !ECMA_COMPAT
				ManualResetEvent ev = new ManualResetEvent(false);
				snakeTimer.Dispose(ev);
			#else
				snakeTimer.Dispose();
			#endif

				// Wait for the system to settle.
				Thread.Sleep(200);
			}

	// The main key processing loop for the game.
	private void MainLoop()
			{
				for(;;)
				{
					ConsoleKeyInfo key = Console.ReadKey(true);
					if(key.KeyChar == '\u001B' || key.KeyChar == 'q' ||
					   key.KeyChar == 'Q' || key.Key == ConsoleKey.Escape)
					{
						// Quit the program.
						break;
					}
					if(gameOver)
					{
						// Start a new game when the user presses any key.
						NewGame();
						DrawBoard();
						SetupTimers();
						continue;
					}
					if(key.Key == ConsoleKey.LeftArrow ||
					   key.KeyChar == 'j' || key.KeyChar == 'J')
					{
						direction = Direction.Left;
					}
					else if(key.Key == ConsoleKey.RightArrow ||
					        key.KeyChar == 'k' || key.KeyChar == 'K')
					{
						direction = Direction.Right;
					}
					else if(key.Key == ConsoleKey.UpArrow ||
					        key.KeyChar == 'i' || key.KeyChar == 'I')
					{
						direction = Direction.Up;
					}
					else if(key.Key == ConsoleKey.DownArrow ||
					        key.KeyChar == 'm' || key.KeyChar == 'M')
					{
						direction = Direction.Down;
					}
					else if(key.KeyChar == 'n' || key.KeyChar == 'N')
					{
						// User requested a new game.
						NewGame();
						DrawBoard();
						SetupTimers();
					}
					else if(key.KeyChar == 'r' || key.KeyChar == 'R' ||
					        key.KeyChar == '\u0012' || key.KeyChar == '\u000C')
					{
						// User requested a repaint.
						DrawBoard();
					}
					else if(key.Key == (ConsoleKey)0x1200)
					{
						// The window size has changed: start a new game.
						NewGame();
						DrawBoard();
						SetupTimers();
					}
					else if(key.Key == (ConsoleKey)0x1201)
					{
						// Resumed after a process suspension.
						DrawBoard();
					}
				}
			}

}; // class Snake

#else

public class Dummy
{
	public static void Main(String[] args)
	{
		Console.WriteLine
			("This program won't work in the current pnetlib configuration.");
		Console.WriteLine
			("Recompile pnetlib with the correct profile settings.");
	}
}

#endif
