#include <stdio.h>
#include <string.h>

#include <staticlibinit_gcce.h>

#include "sqlite3.h"


int main(void)
{
	printf("Open C DLL Test for sqlite!\n");
	printf("Sqlite Version %s!\n", sqlite3_libversion());
	
/*	FILE *fp = fopen("C:\\data\\redfivelabs\\temp\\test.txt", "a+b");
	if (fp != NULL)
		{
			printf("File opened.\n");
			char *test= "Hallo Welt";
			fwrite(test, strlen(test), 1, fp);
			fclose(fp);
			printf("File closed.\n");
		}
	else
		{
			printf("File cannot be opened.");
		}*/
	sqlite3 *db = NULL;
	int result = sqlite3_open("C:\\data\\redfivelabs\\temp\\test.sqlite", &db);
			
	printf("result = %d\n", result);

//	printf("result = %d\n", sqlite3_openTest("C:\\data\\redfivelabs\\temp\\test2.sqlite"));
	char tmp [512];
	char sql[512];
	char *ptr = tmp;
	result = sqlite3_exec(db, "pragma synchronous = off", NULL, NULL, &ptr);
	printf("result of sqlite3_exec = %d\n", result);

	strcpy(sql, "create table mytable (src_text, dst_float, time_float, value_float)");
	sqlite3_stmt *statement = NULL;
	result = sqlite3_prepare(db, (const char*)sql, (int)strlen(sql), &statement, (const char **)&ptr);
	printf("result of sqlite3_prepare = %d\n", result);
	result = sqlite3_busy_timeout(db, 60000);
	printf("result of sqlite3_busy_timeout = %d\n", result);
	result = sqlite3_step(statement);
	printf("result of sqlite3_step = %d\n", result);
	result = sqlite3_finalize(statement);
	printf("result of sqlite3_finalize = %d\n", result);

	result = sqlite3_exec(db, "begin transaction", NULL, NULL, &ptr);
	printf("result of sqlite3_exec = %d\n", result);

	strcpy(sql, "insert into mytable values ('Hallo Welt', 1, 2, 3)");
	result = sqlite3_prepare(db, (const char*)sql, (int)strlen(sql), &statement, (const char **)&ptr);
	printf("result of sqlite3_prepare = %d\n", result);
	result = sqlite3_busy_timeout(db, 60000);
	printf("result of sqlite3_busy_timeout = %d\n", result);
	result = sqlite3_step(statement);
	printf("result of sqlite3_step = %d\n", result);

	result = sqlite3_exec(db, "commit transaction", NULL, NULL, &ptr);
	printf("result of sqlite3_exec = %d\n", result);

	result = sqlite3_finalize(statement);
	printf("result of sqlite3_finalize = %d\n", result);

	result = sqlite3_close(db);
	printf("result of sqlite3_close = %d\n", result);

	printf("Press a character to exit!");
	int c = getchar();
	return 0;
}
 