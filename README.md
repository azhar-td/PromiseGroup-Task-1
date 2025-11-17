# PromiseGroup-Task-1
Task-1: Category Tree (EF vs Stored Procedure)

This console application compares two approaches to build a 4-level category tree:

Using Entity Framework (EF) + LINQ

Using a T-SQL Stored Procedure (recursive CTE)

It prints both trees in the console and shows the time difference between them.



=== EF + LINQ ===
[EF+LINQ] Built 13 nodes in 18 ms
...

=== Stored Procedure ===
[StoredProc] Built 13 nodes in 3 ms

=== Summary ===
EF+LINQ time:     00:00:00.018
StoredProc time:  00:00:00.003
Difference:        00:00:00.015 faster by StoredProc


