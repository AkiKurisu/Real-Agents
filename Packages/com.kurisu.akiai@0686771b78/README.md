# AkiAI
AkiAI 是一个个人向、实验性的游戏AI框架，可用于设计和开发游戏中的NPC、敌人AI等。

## 依赖
1. [AkiBT](https://github.com/AkiKurisu/AkiBT)
2. [AkiGOAP](https://github.com/AkiKurisu/AkiGOAP)

## 架构

分为数据层、决策层、表现层

## 数据层

数据层主要分为世界状态（WorldState）、黑板（BlackBoard）

### 1. WorldState
世界状态和GOAP中的定义一致，用于抽象游戏世界产生的变化至一个状态值（例如“疲惫”:True即表示角色处于“疲惫”的抽象状态），这样决策层只需要了解世界状态，而不需要了解具体的数值

### 2. BlackBoard
黑板是存放公共变量的工具，从而使行动层可以访问相同资源

## 决策层

决策层分为规划器（Planner）、目标（Goal）和行为（Action），AkiAI中使用GOAP来规划任务

详见[GOAP](https://github.com/AkiKurisu/AkiGOAP)

## 表现层
表现层以任务的方式分布，AkiAI中有多种实现来完成任务

### 1. Behavior Tree

行为树本身也可以作为一个独立的任务单元，因为其本身是一个特定任务的虚拟机，处理的好，你可以在AI之间复用它们，如果要使用，推荐使用`AkiBT`中的`BehaviorTreeSO`，运行时实例化来分配给不同的AI。

### 2. Task Sequence

其实本质而言任务序列（也可以是树）和行为树是一个东西，但因为设计目的不一样这里稍作区分，这里任务序列的层级比行为树高一点，因为它是动态生成的（在`AkiBT`中还未提供动态构建行为树的方式）。

### A. Animation Sequence
  
虽然在现代游戏引擎中，动画系统和其他的GamePlay模块通常是解耦的（例如Unity的动画状态机），但在复杂的游戏AI开发中，不可避免得需要将动画和AI行为完美结合在一起，例如处理动态的动画效果时（Unity的动画状态机是静态的，解决该问题通常我们使用`Unity.Timeline`或其他Sequence工具）。AkiAI中使用Unity的`Playable API`提供了动画任务，这样你可以将动画播放也作为一个任务来交给`Proxy`或`Agent`执行了。（当然这是实验性的）

```C#
// Example for creating an animation sequence
// AnimationController => Clip1 => Clip2 => AnimationController
new AnimationSequenceBuilder(animator)
    .Append(animationClip1, 0.4f)
    .Append(animationClip2, 0.2f)
    .SetFadeOut(0.4f)
    .Build()
    .AppendCallBack(()=>Debug.Log("Animation sequence end"))
    .Run()
    .OnCompleted+=()=>Debug.Log("Task sequence complete");
// Or append to existed task sequence
SequenceTask sequence=new(()=>Debug.Log("Task sequence complete"));
new AnimationSequenceBuilder(animator)
    .Append(animationClip1, 0.2f)
    .Build(sequence)
    .Run()
```

### 3. Proxy

`Agent`是AI的代理，`Proxy`这里充当`Agent`的代理，它是提供任务和执行任务的对象，正如其名，代理的任务不会在Agent中执行，而是由外部的`TaskRunner`来执行。

* 适用场景：过场动画，触发时间带来的相同
* 目的：提供被动触发、支持热插拔的AI行为，例如走到一个陷阱，统一执行一个`Task Sequence`，这样就不需要把行为写在`Goal`和`Action`中了

## 如何使用

1. 创建Context接口和Agent分别继承`IAIContext`和`AIAgent`
```C#
public interface ICustomContext : IAIContext
{
    //Your custom GamePlay components and data
    NavMeshAgent NavAgent { get; }
}
public class CustomAgent : AIAgent<ICustomContext>
{
    public override ICustomContext TContext => YourContext;
}
```
1. 创建TaskID类添加`TaskIDHostAttribute`用于存放Task的索引名称
```C#
[TaskIDHost]
public class CustomTasks
{
    //TaskIDHostAttribute provides a popup menu in inspector
    public static string TaskA = "TaskA";
    public static string TaskB = "TaskB";
}
```
1. 继承AIAction和AIGoal

```C#
public abstract class CustomAction : AIAction<ICustomContext> { }
public abstract class CustomGoal : AIGoal<ICustomContext> { }
```

4. Action中控制Task或在Action中执行行为
```C#
public class DoSomething : CustomAction
{
    protected sealed override void SetupDerived()
    {
        Preconditions["CanDo"] = true;
    }
    protected sealed override void SetupEffects()
    {
        Effects["SomeEffect"] = true;
    }
    public sealed override float GetCost()
    {
        return 5;
    }
    //Called after action is selected
    protected sealed override void OnActivateDerived()
    {
        Host.GetTask(CustomTasks.TaskA).Start();
    }
    //Called after action is unselected
    protected sealed override void OnDeactivateDerived()
    {
        Host.GetTask(CustomTasks.TaskA).Stop();
    }
    public sealed override void OnTick() 
    {
        //Do something in action
    }
}
```

5. 编辑器内挂载CustomAgent并添加BehaviorTask，绑定`CustomTasks`中的TaskID
6. (可选)自定义脚本添加``IAITask``


## 路线图
- [ ] 完善任务序列
- [ ] 提供示例项目
- [ ] 可选生成式人工智能作为额外决策层