# SimpleInterpreter
用C#实现一个简单的解释器

该项目的实现会更注重原理，不会过多关注于性能，一些装箱拆箱引起的GC问题后续可以用Native来优化

参考博客:[https://ruslanspivak.com/lsbasi-part1/]

github:[https://github.com/HeJiZi/SimpleInterpreter]

## 文档目录
[1.项目目录](#项目目录)

[2.笔记（实现过程记录）](#笔记)
- [2022-12-17(实现加减乘除解释器)](#2022-12-17)
- [2022-12-18](#2022-12-18)


## 项目目录
```
|-- Core 核心文件夹，在这里实现解释器的核心类
    |-- Interpreter.cs 解释器，负责对token进行算术处理，返回结果值
    |-- Lexer.cs 负责将输入流拆分成不同的token并返回
    |-- Token.cs
|-- Doc 文档及需要用到的图片资源
|-- Program.cs 实现main函数，解释器入口
```
## 笔记
### 2022-12-17
#### 阶段性结果
实现interpreter, lexer, token三个模块,将语法规则描述翻译成代码，完成了能够解析整形加减乘除的解释器

![](Doc/2022-12-17.png)

#### 笔记内容
几个涉及解释器功能模块划分的名词:**token, lexical analyzer, lexeme, parsing, parser**
#### 什么是token？
token是一个具有type 和value 的对象
``` c#
    public Token(TokenType type, Object value)
    {
        this.Type = type;
        this.Value = value;
    }
```
#### 什么是Lexical analyzer(lexer)?
将输入流分割成不同token的模块被称为lexical analyzer

#### 什么是Lexeme？
构成token的字符序列被称为lexeme， 比如'1112'是一个lexeme，对应的token为{type=int, value=1112}

#### Parsing与parser(lexical analyzer)
parser其实是lexical analyzer的另一个名字，作为解释器的一部分，分析输入流构成的部分被称为parsing。

parser关注于token之间的联系,比如负责识别(1+1)是(int->plus->int)还是(int->minus->int)

解释器根据该关系最终返回一个结果。

#### What is syntax diagram?
语法规则的流程图

![syntax_diagram](Doc/syntax_diagram.png)

#### What is context-free grammars?
一种有些类似于正则表达式的语法规则描述，如以下就是一种grammar:
```
expr : factor((Mul|Div)facotr)*
factor : interger
```
每一行都是一条rule，上述的描述里表示我们实现的语言一共有两条rule

每一条rule由两个部分组成，冒号左边的被称为head，右边的被称为body（例子中的`expr`为head， `factor((Mul|Div)facotr)*` 为body）

body和head 又由 terminal 和non-terminal 组成， 像是Mul,Div,Interger的token被称为terminals，expr,factor被称为non-terminals。

左手边的non-terminal又被称为start symbol

\* 代表匹配零次或者多次，在上述例子中表示(Mul|Div)factor的模式出现0次或多次

笔记仅作为个人理解作用，更详细的解释可以看原文图解：[第四章](https://ruslanspivak.com/lsbasi-part4/)

### 2022-12-18
#### 阶段性结果
添加了括号语法，实现了抽象语法树与一元操作符
重新划分了模块，当前解释器的处理流程如下
```
 =========   Token   ==========   AST   ===============
 | Lexer |  -------> | Parser | ------> | Interpreter |
 =========           ==========         ===============
 
```

#### 笔记内容
#### 抽象语法树（AST）与 ParseTree
AST是一种用来描绘语法的树形结构，每一个中间结点都是操作符(operator)，每个叶子结点都是操作数(operand)

![AST](Doc/AST.png)

从图中可以看到，AST没有中间的语法结点 比起ParseTree更加简洁和紧凑。



