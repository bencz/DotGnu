///
/// Copyright (c) 2004 Thong Nguyen (tum@veridicus.com)
///

using System;
using System.Threading;

namespace ThreadsDemo
{
	/// <summary>
	/// The state of a developer.
	/// </summary>
	public enum DeveloperState
	{
		Thinking,
		Waiting,
		Coding
	}

	/// <summary>
	/// Abstract representation of a developer and "coding developer" algorithm.
	/// </summary>
	public abstract class Developer
	{
		/// <summary>
		/// Raised when the developer's state changes.
		/// </summary>
		public event EventHandler StateChanged;
		
		/// <summary>
		/// Thread the developer runs on.
		/// </summary>
		private Thread thread;

		/// <summary>
		/// The controller the developer listens to.
		/// </summary>
		protected DeveloperController m_controller;

		/// <summary>
		/// Name of the developer.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}
		private string m_Name;

		/// <summary>
		/// Gets the count of how many times the developer has entered the Coding state.
		/// </summary>
		public virtual int CodingCount
		{
			get
			{
				return m_CodingCount;
			}
		}
		private int m_CodingCount;

		/// <summary>
		/// Gets the developer's state.
		/// </summary>
		public virtual DeveloperState State
		{
			get
			{
				return m_state;
			}
		}
		private DeveloperState m_state;

		/// <summary>
		/// Sets the developer's state.
		/// </summary>
		/// <param name="state">The new state</param>
		protected virtual void SetState(DeveloperState state)
		{
			lock (this)
			{
				m_state = state;

				if (state == DeveloperState.Coding)
				{
					m_CodingCount++;
				}

				OnStateChanged();
			}
		}

		/// <summary>
		/// Gets a description of the developer.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} ({1}) [{2}]", Name, State, CodingCount);
		}

		/// <summary>
		/// Raises a StateChanged event.
		/// </summary>
		protected virtual void OnStateChanged()
		{
			if (StateChanged != null)
			{
				StateChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// The developer to the immediate left of the current developer.
		/// </summary>
		public Developer Left
		{
			get
			{
				return m_Left;
			}
			set
			{
				m_Left = value;
			}
		}
		protected Developer m_Left;

		/// <summary>
		/// The developer to the immediate right of the current developer.
		/// </summary>
		public Developer Right
		{
			get
			{
				return m_Right;
			}
			set
			{
				m_Right = value;
			}
		}
		protected Developer m_Right;
		
		/// <summary>
		/// Gets/Sets the number of milliseconds to a developer sepnds coding.
		/// </summary>
		public virtual int CodingDelay
		{
			get
			{
				return m_CodingDelay;
			}
			set
			{
				m_CodingDelay = value;
			}
		}
		private int m_CodingDelay;

	        /// <summary>
        	/// Gets/Sets the number of milliseconds to a developer spends thinking.
	        /// </summary>
		public virtual int ThinkingDelay
		{
			get
			{
				return m_ThinkingDelay;
			}
			set
			{
				m_ThinkingDelay = value;
			}
		}
		private int m_ThinkingDelay;

		/// <summary>
		/// Gets/Sets whether the coding/thinking delay should be a random
		/// number within the range or the exact delay specified.
		/// </summary>
		public virtual bool UseRandomDelay
		{
			get
			{
				return m_UseRandomDelay;
			}
			set
			{
				m_UseRandomDelay = value;
			}
		}
		private bool m_UseRandomDelay;

		/// <summary>
		/// Implementers should return when both the keyboard & mouse have been picked up.
		/// </summary>
		protected abstract void Pickup();

		/// <summary>
		/// Implementers should release both the keyboard & mouse and return.
		/// </summary>
		protected abstract void Putdown();

		/// <summary>
		/// Construct a new developer.
		/// </summary>
		protected Developer(string name, Developer left, Developer right, DeveloperController controller)
		{
			m_Left = left;
			m_Right = right;
			m_controller = controller;
			m_Name = name;
			m_CodingDelay = 1000;
			m_ThinkingDelay = 1000;
			m_CodingCount = 0;
			m_UseRandomDelay = true;

			m_controller.Start += new EventHandler(Controller_Start);
			SetState(DeveloperState.Thinking);
		}

		/// <summary>
		/// Invoked by the controller when we should start.
		/// </summary>
		protected virtual void Controller_Start(object sender, EventArgs eventArgs)
		{
			if (thread == null)
			{
				thread = new Thread(new ThreadStart(Run));

				thread.IsBackground = true;

				thread.Start();
			}
		}

		/// <summary>
		/// Thread run procedure.
		/// </summary>
		private void Run()
		{
			Random random = new Random();

			for (;;)
			{
				// Try to aquire the keyboard & mouse.
				Pickup();

				// Developer is coding here..
				if (UseRandomDelay)
				{
					Thread.Sleep(random.Next(CodingDelay));
				}
				else
				{
					Thread.Sleep(CodingDelay);
				}

				// Put down keyboard and mouse
				Putdown();
			
				// Developer is thinking here..
				if (m_UseRandomDelay)
				{
					Thread.Sleep(random.Next(ThinkingDelay));
				}
				else
				{
					Thread.Sleep(ThinkingDelay);
				}				
			}
		}
	}
}
