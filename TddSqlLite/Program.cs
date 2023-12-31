// See https://aka.ms/new-console-template for more information

using TddSqlLite;

var repl = new Repl(new ConsoleWriteLineWrapper(), new ConsoleInputWrapper(), @"database.txt");

repl.Start();
