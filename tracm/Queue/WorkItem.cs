using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace tracm.Queue
{
	public class ProgressState
	{
		private bool m_showProgress = true;
		private volatile int m_progressValue = (int)ProgressValue.Minimum;
		private State m_progressState = State.NotStarted;
		public delegate void ProgressChanged();
		public event ProgressChanged ProgressChangedEvent;
		private string m_errorString = null;

		public enum State
		{
			NotStarted,
			Running,
			Cancelling,
			Cancelled,
			Error,
			Complete
		}

		public enum ProgressValue : int
		{
			Minimum = 0,
			Running = 50,
			Maximum = 100
		}

		public bool ShowProgressBar
		{
			get { return m_showProgress; }
			set
			{
				if (m_progressState == State.NotStarted)
					m_showProgress = value;
			}
		}

		public int CurrentValue
		{
			get { return m_progressValue; }
			set
			{
				if (value < 0 || value > 100)
					throw new ArgumentOutOfRangeException("CurrentValue", "Must be between 0 and 100");
				m_progressValue = value;
				Changed();
			}
		}

		private void Changed()
		{
			try
			{
				if (ProgressChangedEvent != null)
					ProgressChangedEvent();
			}
			catch { }
		}

		public string CurrentState
		{
			get
			{
				string result = String.Empty;
				switch (m_progressState)
				{
					case State.NotStarted:
						result = "Waiting to start";
						break;
					case State.Running:
						if (ShowProgressBar)
							result = String.Empty;
						else
							result = "Running";
						break;
					case State.Cancelling:
						result = "Cancelling";
						break;
					case State.Cancelled:
						result = "Cancelled";
						break;
					case State.Error:
						result = "Error";
						break;
					case State.Complete:
						result = "Done";
						break;
				}
				return result;
			}
		}

		public void Start()
		{
			m_progressValue = (int)ProgressValue.Minimum;
			m_progressState = State.Running;
			Changed();
		}

		public void Cancel()
		{
			if (m_progressState == State.Running)
			{
				m_progressState = State.Cancelling;
				Changed();
			}
			else if (m_progressState == State.NotStarted)
			{
				m_progressState = State.Cancelled;
				Changed();
			}
		}

		public void Error(string errorString)
		{
			m_progressState = State.Error;
			m_errorString = errorString;
			Changed();
		}

		public void Finish()
		{
			m_progressValue = (int)ProgressValue.Maximum;
			if (m_progressState == State.Cancelling)
				m_progressState = State.Cancelled;
			else if(m_progressState != State.Error)
				m_progressState = State.Complete;
			Changed();
		}

		public bool IsStarted
		{
			get
			{
				bool result = m_progressState != State.NotStarted;
				return result;
			}
		}

		public bool IsDone
		{
			get
			{
				bool result = (m_progressState == State.Cancelled || m_progressState == State.Complete || m_progressState == State.Error);
				return result;
			}
		}

		public bool IsRunning
		{
			get
			{
				bool result = m_progressState == State.Running;
				return result;
			}
		}

		public bool WasCancelled
		{
			get
			{
				bool result = m_progressState == State.Cancelled;
				return result;
			}
		}

		public bool HasError
		{
			get
			{
				bool result = m_progressState == State.Error;
				return result;
			}
		}

		public string ErrorText
		{
			get
			{
				return m_errorString;
			}
		}
	}

	public abstract class WorkItem : INotifyPropertyChanged
	{
		private volatile bool m_run = true;
		private ProgressState m_progress = null;
		private WorkItem m_next = null;
		private DateTime m_startTime = DateTime.MinValue;
		private string m_action = String.Empty;
		private delegate void WorkDelegate();
		public delegate void WorkCompleted();
		public event WorkCompleted WorkCompletedEvent;

		protected abstract void WorkMethod();

		public WorkItem()
		{
			m_progress = new ProgressState();
			m_progress.ProgressChangedEvent += new ProgressState.ProgressChanged(m_progress_ProgressChangedEvent);
		}

		void m_progress_ProgressChangedEvent()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("Progress"));

				string previousAction = m_action;
				if (m_progress.IsRunning)
					m_action = "Cancel";
				else
					m_action = String.Empty;
				if (m_action != previousAction)
					PropertyChanged(this, new PropertyChangedEventArgs("Action"));
			}
		}

		public void Work()
		{
			// don't allow to start a second time
			if (m_progress.IsStarted)
				return;

			WorkDelegate d = new WorkDelegate(DoWork);
			m_startTime = DateTime.Now;
			m_progress.Start();
			d.BeginInvoke(new AsyncCallback(CallbackMethod), d);
		}

		private void DoWork()
		{
			try
			{
				WorkMethod();
			}
			catch (Exception ex)
			{
				m_progress.Error(ex.Message);
			}
		}

		/// <summary>
		/// Called when the delegate finishes
		/// </summary>
		/// <param name="result"></param>
		private void CallbackMethod(IAsyncResult result)
		{
			m_progress.Finish();
			WorkDelegate d = (WorkDelegate)result.AsyncState;
			d.EndInvoke(result);

			if (WorkCompletedEvent != null)
				WorkCompletedEvent();

			if (m_next != null)
			{
				if (m_progress.WasCancelled == false)
					m_next.Work();
				else
					m_next.Cancel();
			}
		}

		public void Cancel()
		{
			m_run = false;
			m_progress.Cancel();

			if (m_next != null)
				m_next.Cancel();
		}

		public override string ToString()
		{
			return this.GetType().Name;
		}

		/// <summary>
		/// The name or description of this WorkItem
		/// </summary>
		public string Name
		{
			get { return this.ToString(); }
		}

		/// <summary>
		/// The ProgressState that describes the current state and progress of this WorkItem
		/// </summary>
		public ProgressState Progress
		{
			get { return m_progress; }
		}

		/// <summary>
		/// A label for the action column, either nothing or Cancel
		/// </summary>
		public string Action
		{
			get { return m_action; }
		}

		/// <summary>
		/// Returns true if this WorkItem is still running, false if it is to be cancelled
		/// </summary>
		[Browsable(false)]
		protected bool IsRunning
		{
			get { return m_run; }
		}

		/// <summary>
		/// A shortcut to the integer progress position (0-100)
		/// </summary>
		[Browsable(false)]
		public int ProgressValue
		{
			get { return m_progress.CurrentValue; }
			set { m_progress.CurrentValue = value; }
		}

		/// <summary>
		/// This is used to set any chained WorkItems to start when this WorkItem completes
		/// </summary>
		[Browsable(false)]
		public WorkItem NextItem
		{
			get { return m_next; }
			set { m_next = value; }
		}

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
}
