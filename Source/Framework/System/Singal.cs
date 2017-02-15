﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpenGLF
{
    public static class Singal
    {
        static Dictionary<string, List<ISingalable>> _registerSingalMap = new Dictionary<string, List<ISingalable>>();
        
        public static void registerSingalTrigger(string singalTrigger,ISingalable callbackObject)
        {
            if (callbackObject != null && singalTrigger.Length != 0)
            {
                if (!_registerSingalMap.ContainsKey(singalTrigger))
                {
                    _registerSingalMap.Add(singalTrigger, new List<ISingalable>());
                }
                _registerSingalMap[singalTrigger].Add(callbackObject);
            }
        }

        public static void registerSingalTrigger(string singalTrigger, SingalCallBackWrapper.OnSingalCallBackFunc func){
            registerSingalTrigger(singalTrigger, new SingalCallBackWrapper(func));
        }

        public static void sendSingal(string singalTrigger,object param,bool isAsync = false)
        {
            if (!_registerSingalMap.ContainsKey(singalTrigger))
                return;
            if (!isAsync)
                _executeSendSingal(singalTrigger, param);
            else
                ThreadPool.QueueUserWorkItem((state) => {
                    _executeSendSingal(singalTrigger, param);
                }, null);
        }

        private static void _executeSendSingal(string singalTrigger, object param)
        {
            List<ISingalable> callbackList = _registerSingalMap[singalTrigger];
            _registerSingalMap[singalTrigger] = new List<ISingalable>();

            foreach (var callbackObj in callbackList)
                callbackObj.onSingle(singalTrigger, param);
        }
    }

    public interface ISingalable
    {
        void onSingle(string singalTrigger, object param);
    }

    public sealed class SingalCallBackWrapper : ISingalable
    {
        public delegate void OnSingalCallBackFunc(string singalTrigger, object param);

        OnSingalCallBackFunc _func;

        public SingalCallBackWrapper(OnSingalCallBackFunc func)
        {
            _func = func;
        }

        public void onSingle(string singalTrigger, object param)
        {
            _func(singalTrigger, param);
        }
    }
}