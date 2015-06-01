## Provide a way to write macros on the same language, that you use for writing your code ##
Nemerle simple code
```
static power (x : double, n : int) : double
 {
   if (n == 0)
     1.0
   else
     if (n % 2 == 0) // even
       Sqr (power (x, n / 2))
     else
       x * power (x, n - 1)
 }
```
And nemerle macros sample
```
macro castedarray (e) {
 match (e) {
  | <[ array [.. $elements ] ]> =>
     def casted = List.Map (elements, fun (x) { <[ ($x : object) ]> });
     <[ array [.. $casted] ]>
  | _ => e
 }
}
```
is is two different languages, with different syntax
## Do not force user to learn a new huge custom API, consider KISS principle ##
Boo Ast Macros - you should learn Api of custom AST parser, etc
```
    override def Expand(macro as MacroStatement) as Statement:
        assert 1 == macro.Arguments.Count
        assert macro.Arguments[0] isa ReferenceExpression

        inst = macro.Arguments[0] as ReferenceExpression

        // convert all _<ref> to inst.<ref>
        block = macro.Block
        ne = NameExpander(inst)
        ne.Visit(block)
        return block
```