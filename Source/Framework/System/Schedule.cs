using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenGLF
{
    public static class Schedule
    {
        private static bool _isInit = false;

        //public static bool IsInit { get { return _isInit; } }

        static List<ScheduleTask> _taskList = new List<ScheduleTask>();

        static List<ScheduleTask> _mainThreadTaskList = new List<ScheduleTask>();

        static Stopwatch _watch = new Stopwatch();

        static Thread _thread;

        static bool _endLoop = false;

        static object _lock = new object();

        public class ScheduleTask
        {
            internal bool _isLoop=false;
            internal float _time = 0;
            internal bool _isPick = false;
            internal bool _ableDistory = false;
            internal float _waitTime = 0;
            internal float _currentTime = 0;
            internal object _state = null;

            internal bool _loopDone = false;

            public bool LoopDone { get { return _loopDone; } }

            public object Param { get { return _state; } }

            /// <summary>
            /// 新建一个一次性的计时任务
            /// </summary>
            /// <param name="waitTime">等待时间</param>
            /// <param name="initFunction">触发回调</param>
            /// <param name="param">携带自定义参数</param>
            public ScheduleTask(long waitTime,ScheduleFunction initFunction,object param) : this(waitTime, false, param, 0,initFunction) { }

            /// <summary>
            /// make a task and add into Schedule for managering time to call;
            /// </summary>
            /// <param name="waitTime">time to call</param>
            /// <param name="isLoop">is task call once and remove it from manager</param>
            /// <param name="state">param to your callback function</param>
            /// <param name="loopTimes">loop times,it's vaild when "isLoop=true,-1 means endless loop"</param>
            /// <param name="initFunction">time to call function</param>
            public ScheduleTask(long waitTime,bool isLoop,object param = null,int loopTimes=-1,ScheduleFunction initFunction=null)
            {
                _waitTime = waitTime;
                _isLoop = isLoop;
                _time = loopTimes;
                _state = param;

                if (initFunction != null)
                    onScheduleCallEvent += initFunction;

                resetLoopTime();
            }

            public virtual void onSchedule(object state)
            {
                onScheduleCallEvent(this,state);
            }

            public event ScheduleFunction onScheduleCallEvent;
            public delegate void ScheduleFunction(ScheduleTask refTask,object param);

            /// <summary>
            /// 跳过剩余的循环次数，终止循环
            /// </summary>
            public void markLoopDone()
            {
                _loopDone = true;
            }

            internal void resetLoopTime()
            {
                _currentTime = _waitTime;
            }
        }

        public static void init()
        {
            if (_isInit)
                return;

            _thread = new Thread(threadRun);
            _thread.IsBackground = true;
            _thread.Start();
            _watch.Start();
        }

        /// <summary>
        /// 删除在计划中的任务
        /// </summary>
        /// <param name="task"></param>
        public static void removeTask(ScheduleTask task)
        {
            lock (_lock)
            {
                _taskList.Remove(task);
            }
        }

        /// <summary>
        /// 添加一个任务，到时会异步回调
        /// </summary>
        /// <param name="task"></param>
        public static void addTask(ScheduleTask task)
        {
            lock (_lock)
            {
                _taskList.Add(task);
            }
        }

        /// <summary>
        /// 添加一个任务，会在引擎主线程更新并回调
        /// </summary>
        /// <param name="task"></param>
        public static void addMainThreadUpdateTask(ScheduleTask task)
        {
            _mainThreadTaskList.Add(task);
        }

        /// <summary>
        /// 删除在主线程更新任务的任务
        /// </summary>
        /// <param name="task"></param>
        public static void removeMainThreadUpdateTask(ScheduleTask task)
        {
            _mainThreadTaskList.Remove(task);
        }

        private static void executeTask(ScheduleTask task)
        {
            task._isPick = true;
            ThreadPool.QueueUserWorkItem(_executeTaskEx,task);
        }

        private static void _executeTaskEx(object state)
        {
            ScheduleTask task = (ScheduleTask)state;
            task.onSchedule(task.Param);
            if (!task._isLoop)
            {
                task._ableDistory = true;
            }
            else
            {
                task._time=task._time<0?-1:task._time-1;

                if((task._time==0)||(task._loopDone))
                    task._ableDistory = true;
            }

            task.resetLoopTime();

            task._isPick = false;
        }

        internal static void mainThreadRun()
        {
            long passTime = _watch.ElapsedMilliseconds;
            //Console.WriteLine("mainThread watch"+passTime);
            _watch.Restart();
            ScheduleTask task;

            for (int i = 0; i < _mainThreadTaskList.Count; i++)
            {
                task = _mainThreadTaskList[i];

                //Task is already to remove
                if (task._ableDistory)
                {
                    removeMainThreadUpdateTask(task);
                    i--;
                    continue;
                }

                task._currentTime -= passTime;
                if (task._currentTime <= 0)
                {
                    _executeTaskEx(task);
                }
                //Console.WriteLine(00);
            }

            Thread.Sleep(2);
        }

        private static void threadRun()
        {
            ScheduleTask task;
            Stopwatch watch = new Stopwatch();
            long passTime;
            watch.Start();
            while (!_endLoop)
            {
                passTime = watch.ElapsedMilliseconds;
                //Console.WriteLine("watch"+passTime);
                watch.Restart();

                for(int i = 0; i < _taskList.Count; i++)
                {
                    task = _taskList[i];

                    //Task is already to remove
                    if (task._ableDistory)
                    {
                        removeTask(task);
                        i--;
                        continue;
                    }

                    //Task is processing
                    if (task._isPick)
                        continue;

                    task._currentTime -= passTime;
                    if (task._currentTime <= 0)
                        executeTask(task);

                    //Console.WriteLine(00);
                }

                Thread.Sleep(2);
            }
        }
    }
}
