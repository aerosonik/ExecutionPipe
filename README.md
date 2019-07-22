<img src="https://raw.githubusercontent.com/aerosonik/ValidationPipe/f5997cdfaff661d36939c45823e93bb613a3767d/icon.png" alt="fo-dicom logo" height="80" />

# ExecutionPipe
Lightweight library with base abstractions and implementations for creating custom pipe of sequential or parallel  excecutions.

[![NuGet](https://img.shields.io/nuget/v/NSV.ExecutionPipe.svg)](https://www.nuget.org/packages/NSV.ExecutionPipe)
[![Build status](https://ci.appveyor.com/api/projects/status/r3yptmhufh3dl1xc?svg=true)](https://ci.appveyor.com/project/aerosonik/validationpipe)

### Installation

Get latest stable version from [NuGet](https://www.nuget.org/packages/NSV.ExecutionPipe/).

## General purpose
* Use it as part of application architecture.
* Use it to get more control of your business process implementations, instead of creating set of methods and complex logic of theirs interactions use this library and receive simple and understandable pipe that fully describes your process.
* Decrease complexity of business logic implementations in case when application has logic layer which consists of many sub layers that eventually becomes into unmanageable code. It's easy to put many parts from differnt layers of one logical chain into one line (but it can be a tree).
* It's fully match to S.O.L.I.D., D.R.Y. and K.I.S.S. principles
* Design Patterns - it use "Strategy" pattern to describe chain of executors, and custom implementation of "Builder" pattern.


## Main features
* Class library with completed set of abstractions
* Targets .NET Standard 2.0 and higher
* Whole syntax is FLUENT
* High-performance, fully asynchronous `async`/`await` API
* Execute `Pipe` sequential or able to execute parallel
* Full control of execution process
* Embeded StopWatch for whole Pipe and for each Executor
* Shared threadsafe local cache for `Pipe` and included Executors and Pipes
* `Pipe` consists of executors
* `Pipe` can contain nested `Pipe` (Pipe implement `IExecutor)
* `Pipe` supports nested pipes and nested `Pipe` also supports nested pipes
* `Executor` can be marked by `Label` for distinguish set of result

## Memory optimized usage:
`Pipe` based on queue data structure, each executor will be removed from queue after invocation, it's a good chance to let objects die in zero generation.

### Base abstractions :

#### Pipe<M, R>
Base Pipe abstraction, implement it to create your own Pipe of execution. It will make sense if you set up pipe by using embedded functionality, adding executors, setting execution mode.

M - model, R - result

Members:
*   `AsParallel()`  - all executors in `Pipe` will execute in parallel mode
*   `AsSequential()` - default behaviour, `Pipe` will execute all executors step by step
*   `UseStopWatch()` - use for measure execution `Pipe` time, see result in `Elapsed` property
*   `UseLocalCache()` - initialised local cache, for using by all `Pipe` members (executors, pipes)
*   `UseLocalCacheThreadSafe()` - the same, but it can be used in parallel mode
*   `If(bool condition)` - if condition is True, executors between `If(..)` and `EndIf()` will be added to pipe, otherwise they will not be added.
*   `If(Func<M, bool> condition)` - this mean : if "condition" returns True,  executors between `If(..)` and `EndIf()` will be invoceted.
*   `EndIf()` - this operator close the "If" section. (every If should be ended with "EndIf") 
*   `Execute(M model)` - use it in case when all executors are sync
*   `ExecuteAsync(M model)` - use it in case when some of executors are async, you can mix sync and async executors
*   `Finish()` - use to finish customising your `Pipe`
*   `CreateResult(M model, PipeResult<R>[] results)` - implelemt it to handle all results from all executors, this method will called in end of execution chain

##### ISequentialPipe<M, R>, IParallelPipe<M, R>
Pipe implement ISequentialPipe<M, R>, IParallelPipe<M, R>

Method `Pipe.AsSequential()` return `ISequentialPipe<M, R>`, use to setup sequence of executors

Method `Pipe.AsParallel()` return `IParallelPipe<M, R>`, use to sutup parallel execution of executors

*   `AddExecutor(IExecutor<M, R> executor)` - add current executor (implementation of abstract class `Executor`)
*   `AddExecutor(Func<IExecutor<M, R>> executor)` - add executor - use it for lazy initialization of executor, the executor will be initialized just before invocation of `Execute(..)` or `ExrcuteAsync(..)`methods.
*   `SetSkipIf(Func<M, bool> condition)` - if result of condition is true, executor will be skipped
*   `SetBreakIfFailed()` - use to break sequence of execution if current executor returns failed result (only in `ISequentialPipe<M, R>`)
*   `SetAllowBreak()` - use it if you need to break sequence on current `Executor` which simultaneously returned successful result and `Break` marker (only in `ISequentialPipe<M, R>`)
*   `SetResultHandler(Func<M, PipeResult<R>, PipeResult<R>> handler)` - use this method to handle result from current executor in case when sequence will break after it (only in `ISequentialPipe<M, R>`)
*   `SetUseStopWatch()` - use it to determine that the current executor will count it's own time of execution
*   `SetLabel(string label)` - set label to differ results
*   `SetRetryIfFailed(int count, int timeOutMilliseconds)` - execution of current executor can be repeated `count`-times, in timeOutMilliseconds each time (only in `ISequentialPipe<M, R>`)

#### Executor
Abstract class, implement this class to create your own `Executor`
*   `Execute(M model)` - abstract method, to use `Executor` in `Pipe` implement it
*   `ExecuteAsync(M model)` - abstract method, to use `Executor` in `Pipe` implement it, in case your execution is asynchronious 
*   `IsAsync` - by default is `true`, and `ExecuteAsync(M model)` method will be executed, if you implement synchronious method set  `IsAsync` to `false`

## Usage
For example, consider process that consists of several simple stages where every next stage depends on result of executed previous stage, let's implement Executor for each stage, and then  implement `Pipe` and put all executors into sequence of this pipe.

#### Model and Result
```csharp
public class ProcessModel //input model
{
  public int Id { get; set; }
  public string Text { get; set; }
}

public class ProcessResult
{
  public string ResultField { get; set; }
}
```
#### Executors
Using `ProcessModel` and `ProcessResult` as generics arguments.
`Executor` is base work unit in pipe, all units use the same type of model and same instance of model object, they return the same type of results, this result `ProcessResult` will be returned after execution as `Value` of struct `PipeResult<R>`.
```csharp
public class ProcessExecutor1 : Executor<ProcessModel, ProcessResult>
{
  public ProcessExecutor1()
  {
    IsAsync = false;  // by default is True
  }
  // implemented, because IsAsync is false
  public override PipeResult<ProcessResult> Execute(ProcessModel model)
  {
    Thread.Sleep(2000); // imitation of work
    return PipeResult<ProcessResult> // return result
    .DefaultSuccessful // helper of structure initialization
    .SetValue(new ProcessResult { ResultField = "First result" }); // The value result
  }
  public override Task<PipeResult<ProcessResult>> ExecuteAsync(ProcessModel model)
  {
    throw new NotImplementedException();
  }
}

public class ProcessExecutor2 : Executor<ProcessModel, ProcessResult>
{
  // IsAsync = true; can be simplified
  public override PipeResult<ProcessResult> Execute(ProcessModel model)
  {
    throw new NotImplementedException();
  }
  // implemented, because IsAsync is true, default behaviour
  public override async Task<PipeResult<ProcessResult>> ExecuteAsync(ProcessModel model)
  {
    await Task.Delay(3000); // imitation of work
    return PipeResult<ProcessResult>
      .DefaultUnSuccessful // helper of structure initialization
      .SetValue(new ProcessResult { ResultField = "Second result" });
  }
}
```
#### Pipes
Sequential execution
```csharp
public class ProcessPipe : Pipe<ProcessModel, ProcessResult>
{
  public ProcessPipe(ProcessExecutor1 executor1, 
     ProcessExecutor2 executor2, 
     ProcessExecutor3 executor3, 
     ProcessSubPipe subPipe) // ProcessSubPipe - should be inherited from "Pipe<ProcessModel, ProcessResult>"
  {
    UseLocalCache() // 
    .UseStopWatch() // get info about pipe execution time
    .AsSequential() // all executors on this level will executed sequentially, it doesn't affect on any included subpipes
    .AddExecutor(executor1) //add executor to pipe, first in invocation queue
      .SetLabel("Label of ProcessExecutor1") // any string
      .SetBreakIfFailed() // allow break sequense of executors if this one failed
      .SetResultHandler((model, result) => { return model.Text != "Text" ? result : result.SetError("Text is default"); }) // quit without invocation method 'CreateResult'
      .SetSkipIf(m => m.Id == 0) // this executor (executor1) willnot be executed if condition is true
    .AddExecutor(subPipe)// just sub pipe with same generic args, can contain any executors of the same generics args
      .SetSkipIf(m => m.Id <= 1) // will be executed if 'Id'  greater than 1
    .AddExecutor(executor2) //add executor to pipe, second in invocation queue
      .SetLabel("Label of ProcessExecutor2") // any string
      .SetRetryIfFailed(3, 1000) // retry invoke 'executor2' 3 times with 1 second delay betwen attempts
      .SetAllowBreak() // allow break when result is successful anf flag 'Break' is true
    .AddExecutor(executor3) //add executor to pipe, third in invocation queue
      .SetLabel("ProcessExecutor3") // any string
      .SetUseStopWatch() // get info about invocation time, returned in result
    .Finish();
  }

  public override PipeResult<ProcessResult> CreateResult(ProcessModel model, PipeResult<ProcessResult>[] results)
  {
    var pipeTime = Elapsed; // // stopwatch result on execution of this pipe, setuped by '.UseStopWatch()'
    var time = results.FirstOrDefault(x => x.Label == "ProcessExecutor3").Elapsed; // stopwatch result on invocation of 'executor3', setuped by '.SetUseStopWatch()'
    switch (results.AllSuccess())
    {
      case ExecutionResult.Successful:
        return PipeResult<ProcessResult>.DefaultSuccessful;
      case ExecutionResult.Failed:
        return PipeResult<ProcessResult>.DefaultUnSuccessful;
      default:
        return PipeResult<ProcessResult>.Default;
    }
  }
}
```
Parallel execution
```csharp
public class ProcessPipe : Pipe<ProcessModel, ProcessResult>
{
  public ProcessPipe(ProcessExecutor1 executor1, 
     ProcessExecutor2 executor2, 
     ProcessExecutor3 executor3, 
     ProcessSubPipe subPipe)
  {
    UseLocalCache() // 
    .UseStopWatch() // get info about pipe execution time
    .AsParallel() // all executors on this level will executed parallel, it doesn't affect on any included subpipes
    .AddExecutor(executor1) //add executor to pipe
      .SetLabel("Label of ProcessExecutor1") // any string
      .SetSkipIf(m => m.Id == 0) // this executor (executor1) willnot be executed if condition is true
    .AddExecutor(subPipe)// this is sub pipe
      .SetSkipIf(m => m.Id <= 0),
    .AddExecutor(executor2) //add executor to pipe
      .SetLabel("Label of ProcessExecutor2") // any string
    .AddExecutor(executor3) //add executor to pipe
      .SetLabel("ProcessExecutor3") // any string
      .SetUseStopWatch() // get info about invocation time, returned in result
    .Finish();
  }
  public override PipeResult<ProcessResult> CreateResult(ProcessModel model, PipeResult<ProcessResult>[] results)
  {
    // aggregate results here
  }
}
```
Run pipe
```csharp
public static void Main()
{
  var model = new ProcessModel { Id = 2, Text = "any text" };// add input data model
  var pipe = new ProcessPipe(
    new ProcessExecutor1(), 
    new ProcessExecutor2(), 
    new ProcessExecutor3(), 
    new ProcessSubPipe()); // create pipe, better to use DI 
  var result = pipe
    .Execute(model); // run baby run!!!
}
```
Or
```csharp
public static async Task Main()
{
  var model = new ProcessModel { Id = 2, Text = "any text" };
  var pipe = new ProcessPipe(.....);
  var result = await pipe
    .ExecuteAsync(model); // run asynchronously
}
```
Use Dependency injection
```csharp
public class Process
{
   private Pipe<ProcessModel,ProcessResult> _pipe;
   public Process(Pipe<ProcessModel,ProcessResult> pipe,
      ProcessExecutor1 exec1,
      ProcessExecutor2 exec2,
      ProcessExecutor3 exec3,
      ProcessExecutor4 exec4)
   {
      _pipe = pipe
         .UseLocalCache()
         .UseStopWatch()
         .AsSequential()
         .AddExecutor(exec1)
         .AddExecutor(exec2)
         .AddExecutor(exec3)
         .AddExecutor(exec4)
         .Finish();
   }
   public bool Run()
   {
      var model = new ProcessModel { Id = 2, Text = "any text" };
      var result = _pipe.Execute(model);
      if(result.Success == ExecutionResult.Successful)
         return true;
      return false;
   }
}
```
Lazy initialization, each executor will be created only before invocation
```csharp
public class Process
{
   private Pipe<ProcessModel,ProcessResult> _pipe;
   public Process(Pipe<ProcessModel,ProcessResult> pipe,
      Func<ProcessExecutor1> exec1,
      Func<ProcessExecutor2> exec2,
      Func<ProcessExecutor3> exec3,
      Func<ProcessExecutor4> exec4)
   {
      _pipe = pipe
         .UseLocalCache()
         .UseStopWatch()
         .AsSequential()
         .AddExecutor(exec1)
         .AddExecutor(exec2)
         .AddExecutor(exec3)
         .AddExecutor(exec4)
         .Finish();
   }
   public bool Run()
   {
      var model = new ProcessModel { Id = 2, Text = "any text" };
      var result = _pipe.Execute(model);
      if(result.Success == ExecutionResult.Successful)
         return true;
      return false;
   }
}
```


### Structure `PipeResult<R>`
It is a pipe result and result returned by all executors in pipe.

Properties:

* `Success` - show result, can be `Initial` - executor/pipe wasn't executed, `Successful` - succsess, `Failed` - error, fail, something went wrong.
* `Errors` - contains `string[]` for errors.
* `Exceptions` - contains `Exception[]` for exceptions.
* `Value` - result value of <`R`> type, place here result object.
* `Elapsed` - time of execution
* `Label` - string label
* `Break` - set true, if You need to break sequence, use it with `SetAllowBreak()`

Static getters:

* `Default` - returns default new value of structure (initial state)
```csharp
return new PipeResult<T>
{
  Value = Optional<T>.Default,
  Break = false,
  Errors = Optional<string[]>.Default,
  Exceptions = Optional<Exception[]>.Default,
  Success = ExecutionResult.Initial
};
```
* `DefaultSuccessful` - returns default value, with `Success = ExecutionResult.Successful`
* `DefaultSuccessfulBreak` - returns default value, with `Success = ExecutionResult.Successful` and `Break = true`
* `DefaultUnSuccessful` - returns default value, with `Success = ExecutionResult.Failed` and `Break = false`
* `DefaultUnSuccessfulBreak` - returns default value, with `Success = ExecutionResult.Failed` and `Break = true`

Fluent Methods:

All these methods return type `PipeResult<R>`, for fluent syntax usage
* `SetValue(T value)` - set `Value` property
* `SetBreak(bool isbreak)` - set `Break` property, use it with `SetAllowBreak()`
* `SetErrors(string[] errors)` - set `Errors` property
* `SetError(string error)` - set `Errors` property with singl error
* `SetException(Exception[] exceptions)` - set property `Exceptions`
* `SetException(Exception exception)` - set property `Exceptions` with singl exception
* `SetSuccessful()` - set property `Success = ExecutionResult.Successful`
* `SetUnSuccessful()` - set property `Success = ExecutionResult.Failed`
* `SetElapsed(TimeSpan span)` - set property  `Elapsed`
* `SetLabel(string label)` - set property `Label`

Extensions : 
* `string[] AllErrors<T>(this PipeResult<T>[] results)` - returns list of errors messages from failed results
* `Exception[] AllExceptions<T>(this PipeResult<T>[] results)` - return list of exeptions from failed results
* `ExecutionResult AllSuccess<T>(this PipeResult<T>[] results)` - return `Success` if all results are successful, `Initial` if all are initial, `Failed` in any other case
* `ExecutionResult AllExecutedSuccess<T>(this PipeResult<T>[] results)` - return `Success` if all results are successful, `Initial` if all are initial, `Failed` in any other case. Method handle only results which are not `Initial`
* `ExecutionResult AnySuccess<T>(this PipeResult<T>[] results)` - return `Success` if any of results is `Success`, if all are in initial state will returned `Initial`, `Failed` in any other case
* `bool IsAllSuccess<T>(this PipeResult<T>[] results)` - return `Success` if all results are in `Success` state
* `bool IsAllExecutedSuccess<T>(this PipeResult<T>[] results)` - return `Success` if all results are in `Success` state, except results wich are in `Initial` state
