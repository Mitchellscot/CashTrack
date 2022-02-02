--dotnet ef commands
--   dotnet ef migrations add init -o .\Data\Migrations

--REMEMBER TO CREATE A SEPERATE TEST DATABASE
--Go to startup and set Configuration.GetConnectionString("DefaultConnection") - change DefaultConnection to TestDB and then 
--dotnet-ef database update and then go back to startup and change it back to DefaultConnection
--also make sure postgresql timezone is set correctly before import

--show timezone;
--set timezone to 'America/Chicago';
--also set it in the .conf file

--better yet just change it to 'America/Danmarkshavn';

--copy data from expense main category file
COPY expense_main_categories(id, main_category_name)
FROM 'C:\Users\Public\db-files\expense-main-categories.csv'
DELIMITER ','
CSV HEADER;

--copy data from expense sub category file
COPY expense_sub_categories(id, sub_category_name, main_categoryid, in_use)
FROM 'C:\Users\Public\db-files\expense-sub-categories.csv'
DELIMITER ','
CSV HEADER;

--copy data from merchants file
COPY merchants(id, name, suggest_on_lookup, city, State)
FROM 'C:\Users\Public\db-files\merchants.csv'
DELIMITER ','
CSV HEADER;

--copy data from expenses file
COPY expenses(purchase_date, amount, categoryid, notes, merchantid, exclude_from_statistics)
FROM 'C:\Users\Public\db-files\expenses.csv'
DELIMITER ','
CSV HEADER;

--copy data from income categories file
COPY income_categories(id, category)
FROM 'C:\Users\Public\db-files\income-categories.csv'
DELIMITER ','
CSV HEADER;

--copy data from income sources file
COPY income_sources(id, source)
FROM 'C:\Users\Public\db-files\income-sources.csv'
DELIMITER ','
CSV HEADER;

--copy data from incomes file
COPY incomes(income_date, amount, categoryid, sourceid, notes)
FROM 'C:\Users\Public\db-files\incomes.csv'
DELIMITER ','
CSV HEADER;

--create some default tags
insert into tags(tag_name)values
('Mitch'),('Sarah'),('Henry'),('Lydia'),('Edward'),('Arthur'),
('Drums'),('Cymbals'),('Drum Hardware'),('Drum Sticks');

--find all rows in 2021
select * from expenses WHERE purchase_date between '2021-01-01'::date and '2021-12-31'::date ORDER BY purchase_date;

--find all groceries
select * from expenses where categoryid = 31;

--select statements
select * from expenses;
select * from expense_main_categories;
select * from expense_sub_categories;
select * from expense_tags;
select * from income_categories;
select * from income_sources;
select * from incomes;
select * from merchants;
select * from tags;
select * from users;

--danger zone

--delete all data in a table
delete from expenses;
delete from expense_main_categories;
delete from expense_sub_categories;
delete from expense_tags;
delete from income_categories;
delete from income_sources;
delete from incomes;
delete from merchants;
delete from tags;
delete from users;

--drop a table
drop table expenses;
drop table expense_main_categories;
drop table expense_sub_categories;
drop table expense_tags;
drop table income_categories;
drop table income_sources;
drop table incomes;
drop table merchants;
drop table tags;
drop table users;

----tag example. Set a tag named "Mitch" to every expense in the dentist category with the name "Mitch" in the notes
--select * from expenses WHERE notes ILIKE '%mitch%' AND categoryid=14;

--insert into tags(tag_name) VALUES ('Mitch') RETURNING id;

----these ExpenseIds might be different. Enter in the ids that you get from the above returning statement
--insert into expense_tags(expense_id, tag_id) VALUES 
--	(659, 1),
--	(1532, 1),
--	(3028, 1),
--	(3048, 1),
--	(3094, 1),
--	(3098, 1),
--	(3108, 1);

----confirm tag works be returning all tags with the tag name Mitch
--select expenses.id, expenses.purchase_date, expenses.amount, expenses.merchantid, expenses.notes, expenses.categoryid
--from expenses
--join expense_tags ON expense_tags.expense_id=expenses.id
--join tags ON expense_tags.tag_id=tags.id
--where tags.tag_name= 'Mitch'
--group by expenses.id, expenses.purchase_date, expenses.amount, expenses.merchantid, expenses.notes, expenses.categoryid
--order by purchase_date ASC;