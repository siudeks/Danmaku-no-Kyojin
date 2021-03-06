﻿using System.Diagnostics;

namespace DanmakuNoKyojin.BulletEngine
{
	/// <summary>
	/// An action task, this dude contains a list of tasks that are repeated
	/// </summary>
	public class BulletMLAction : BulletMLTask
	{
		#region Members

		/// <summary>
		/// The max number of times to repeat this action
		/// </summary>
		private int RepeatNumMax;

		/// <summary>
		/// The number of times this task has been run.
		/// This starts at 0 and the task will repeat until it hits the "max"
		/// </summary>
		private int RepeatNum;

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLAction"/> class.
		/// </summary>
		/// <param name="repeatNumMax">Repeat number max.</param>
		/// <param name="node">Node.</param>
		/// <param name="owner">Owner.</param>
		public BulletMLAction(int repeatNumMax, BulletMLNode node, BulletMLTask owner) : base(node, owner)
		{
			Debug.Assert(null != Node);
			Debug.Assert(null != Owner);
			this.RepeatNumMax = repeatNumMax;
		}

		/// <summary>
		/// Init this task and all its sub tasks. 
		/// This method should be called AFTER the nodes are parsed, but BEFORE run is called.
		/// </summary>
		protected override void Init()
		{
			base.Init();
			RepeatNum = 0;
		}

		/// <summary>
		/// Run this task and all subtasks against a bullet
		/// This is called once a frame during runtime.
		/// </summary>
		/// <returns>ERunStatus: whether this task is done, paused, or still running</returns>
		/// <param name="bullet">The bullet to update this task against.</param>
		public override ERunStatus Run(Bullet bullet)
		{
			//run the action until we hit the limit
			while (RepeatNum < RepeatNumMax)
			{
				ERunStatus runStatus = base.Run(bullet);

				//What was the return value from running all teh child actions?
				switch (runStatus)
				{
					case ERunStatus.End:
					{
						//The actions completed successfully, initialize everything and run it again
						RepeatNum++;
						base.Init();
					}
					break;

					case ERunStatus.Stop:
					{
						//Something in the child tasks paused this action
						return runStatus;
					}

					default:
					{
						//One of the child tasks needs to keep running next frame
						return ERunStatus.Continue;
					}
				}
			}

			//if it gets here, all the child tasks have been run the correct number of times
			TaskFinished = true;
			return ERunStatus.End;
		}

		#endregion //Methods
	}
}