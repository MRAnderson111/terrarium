using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
/// <summary>
/// 计时工具类，用于性能测量和调试，支持按名称同时记录多个任务
/// </summary>
public static class TTimer
{
    private static Dictionary<string, Stopwatch> runningTimers = new Dictionary<string, Stopwatch>();
    /// <summary>
    /// 开始或重置指定任务的计时器
    /// </summary>
    /// <param name="taskName">要计时的任务名称</param>
    public static void StartTimer(string taskName)
    {
        if (string.IsNullOrEmpty(taskName))
        {
            UnityEngine.Debug.LogError("TTimer: Task name cannot be null or empty.");
            return;
        }
        if (runningTimers.TryGetValue(taskName, out Stopwatch stopwatch))
        {
            stopwatch.Reset();
            stopwatch.Start();
        }
        else
        {
            stopwatch = new Stopwatch();
            runningTimers.Add(taskName, stopwatch);
            stopwatch.Start();
        }
    }
    /// <summary>
    /// 停止指定任务的计时器并输出耗时
    /// </summary>
    /// <param name="taskName">要停止计时的任务名称</param>
    public static void StopTimer(string taskName)
    {
        if (string.IsNullOrEmpty(taskName))
        {
            UnityEngine.Debug.LogError("TTimer: Task name cannot be null or empty.");
            return;
        }
        if (runningTimers.TryGetValue(taskName, out Stopwatch stopwatch))
        {
            stopwatch.Stop();
            long ticks = stopwatch.ElapsedTicks;
            double milliseconds = (double)ticks / Stopwatch.Frequency * 1000;
            UnityEngine.Debug.Log($"计时结束: {taskName}, 耗时(ticks): {ticks}, 耗时(毫秒): {milliseconds:F3}");
            runningTimers.Remove(taskName); // 停止后移除
        }
        else
        {
            UnityEngine.Debug.LogWarning($"TTimer: Timer for '{taskName}' was not found or already stopped.");
        }
    }
}