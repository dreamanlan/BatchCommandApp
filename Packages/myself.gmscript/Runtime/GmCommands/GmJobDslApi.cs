using System;
using System.Collections;
using System.Collections.Generic;
using StoryScript;
using StoryScript.DslExpression;

/// <summary>
/// DSL functions for the frame-pumped coroutine job runner.
///
/// These are generic GM APIs, NOT editor/project-specific commands: they live
/// in the shared runtime assembly so BOTH channels expose the same job_start /
/// job_status / job_result / job_cancel / job_list functions. Register() is
/// called once per interpreter, both times from inside the package:
///   * editor channel  -> EditorDslApi.Register(calculator)  [myself.gmscript.Editor]
///   * runtime channel -> ClientGmStorySystem.RegisterCommon(registry)
///
/// Which jobs can actually be started still depends on the factories
/// registered by the editor side (AppEditorJobApi). The factory table is empty
/// in a standalone player, where job_start simply reports "unknown gm job".
/// </summary>
public static class GmJobDslApi
{
    /// <summary>
    /// A tiny channel-agnostic job used to verify the runner's channel mask.
    /// Registered with Both, so it must start from editor, runtime and device.
    /// It yields a few steps (proving frames are actually handed back) and
    /// reports which channels it declares.
    /// </summary>
    public static IEnumerator Probe(string[] args)
    {
        for (int i = 0; i < 3; ++i)
            yield return new GmJobStep(i + 1, 3, "probe step " + (i + 1));
        yield return new GmJobResult("JOB_CHANNEL_PROBE ok channels=Both args=" +
            string.Join(",", args ?? new string[0]));
    }

    /// <summary>Register on a DslCalculator (editor channel).</summary>
    public static void Register(DslCalculator calc)
    {
        if (null == calc) return;
        calc.Register("job_start", "job_start(name[,arg1,arg2,...]) - start a long batch command as a frame-driven coroutine job; returns JOB_STARTED id=N", new ExpressionFactoryHelper<EditorJobStartExp>());
        calc.Register("job_status", "job_status(id) - poll a coroutine job's state and progress", new ExpressionFactoryHelper<JobStatusExp>());
        calc.Register("job_result", "job_result(id) - fetch a finished job's result string", new ExpressionFactoryHelper<JobResultExp>());
        calc.Register("job_cancel", "job_cancel(id) - request cancellation of a running job", new ExpressionFactoryHelper<JobCancelExp>());
        calc.Register("job_list", "job_list() - list all jobs and registered job names", new ExpressionFactoryHelper<JobListExp>());
    }

    /// <summary>Register on a shared API registry (runtime channel / player).</summary>
    public static void Register(DslCalculatorApiRegistry registry)
    {
        if (null == registry) return;
        registry.Register("job_start", "job_start(name[,arg1,arg2,...]) - start a long batch command as a frame-driven coroutine job; returns JOB_STARTED id=N", new ExpressionFactoryHelper<RuntimeJobStartExp>());
        registry.Register("job_status", "job_status(id) - poll a coroutine job's state and progress", new ExpressionFactoryHelper<JobStatusExp>());
        registry.Register("job_result", "job_result(id) - fetch a finished job's result string", new ExpressionFactoryHelper<JobResultExp>());
        registry.Register("job_cancel", "job_cancel(id) - request cancellation of a running job", new ExpressionFactoryHelper<JobCancelExp>());
        registry.Register("job_list", "job_list() - list all jobs and registered job names", new ExpressionFactoryHelper<JobListExp>());
    }

    private static int IdOf(IList<BoxedValue> operands)
    {
        if (operands.Count <= 0) return 0;
        string s = operands[0].GetString();
        int id;
        if (!int.TryParse(s, out id)) {
            s = operands[0].ToString();
            int.TryParse(s, out id);
        }
        return id;
    }

    private static string ArgOf(IList<BoxedValue> operands, int index)
    {
        return operands.Count > index ? operands[index].GetString() : string.Empty;
    }

    public abstract class JobStartExp : SimpleExpressionBase
    {
        protected abstract GmJobChannel Channel { get; }
        protected override BoxedValue OnCalc(DslCalculator calc, IList<BoxedValue> ops)
        {
            string name = ArgOf(ops, 0);
            var args = new List<string>();
            for (int i = 1; i < ops.Count; ++i) args.Add(ArgOf(ops, i));
            int id = GmCoroutineRunner.StartCommand(name, args.ToArray(), Channel);
            return BoxedValue.FromString("JOB_STARTED id=" + id + " name=" + name +
                (args.Count == 0 ? "" : " args=" + string.Join(",", args.ToArray())));
        }
    }

    public sealed class EditorJobStartExp : JobStartExp
    {
        protected override GmJobChannel Channel { get { return GmJobChannel.Editor; } }
    }

    public sealed class RuntimeJobStartExp : JobStartExp
    {
        protected override GmJobChannel Channel { get { return GmJobChannel.Runtime; } }
    }
    public sealed class JobStatusExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calc, IList<BoxedValue> ops)
        {
            return BoxedValue.FromString(GmCoroutineRunner.Status(IdOf(ops)));
        }
    }
    public sealed class JobResultExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calc, IList<BoxedValue> ops)
        {
            return BoxedValue.FromString(GmCoroutineRunner.Result(IdOf(ops)));
        }
    }
    public sealed class JobCancelExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calc, IList<BoxedValue> ops)
        {
            int id = IdOf(ops);
            return BoxedValue.FromString(GmCoroutineRunner.Cancel(id)
                ? "JOB_CANCEL_REQUESTED id=" + id : "JOB_NOT_RUNNING id=" + id);
        }
    }
    public sealed class JobListExp : SimpleExpressionBase
    {
        protected override BoxedValue OnCalc(DslCalculator calc, IList<BoxedValue> ops)
        {
            return BoxedValue.FromString(GmCoroutineRunner.List());
        }
    }
}
