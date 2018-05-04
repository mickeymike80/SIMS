use master
go

drop database sims
go

create database sims
go

use sims
go

create table departments (
	department_id int primary key identity(1, 1),
	department_name varchar(50),
	created_at datetime,
)
go

create table roles ( 
	role_id int primary key identity(1, 1),
	role_name varchar(50),
	description text,
	created_at datetime,
)
go

create table users (
	user_id int primary key identity(1, 1),
	user_university_id int unique,
	department_id int foreign key references departments(department_id),
	role_id int foreign key references roles(role_id),
	user_name varchar(50),
	email varchar(100) unique,
	password varchar(50),
	created_at datetime,
	last_login datetime,
)
go


create table categories (
	category_id int primary key identity(1, 1),
	category_name varchar(50),
	created_at datetime,
)
go

create table academic_years (
	academic_year_id int primary key identity(1, 1),
	academic_year_name varchar(50),
	started_at datetime,
	ended_at datetime,
	deadline_ideas datetime,
	deadline_comments datetime,
)
go

create table ideas (
	idea_id int primary key identity(1, 1),
	user_id int foreign key references users(user_id),
	category_id int foreign key references categories(category_id),
	academic_year_id int foreign key references academic_years(academic_year_id),
	idea_title varchar(50),
	idea_content text,
	viewed_count int,
	isAnonymous tinyint,
	isEnabled tinyint,
	status tinyint,
	created_at datetime
)
go

create table documents (
	document_id int primary key identity(1, 1),
	idea_id int foreign key references ideas(idea_id),
	old_file_name varchar(100),
	new_file_name varchar(100),
	created_at datetime
)
go

create table rates (
	rate_id int primary key identity(1, 1),
	user_id int foreign key references users(user_id),
	idea_id int foreign key references ideas(idea_id),
	rate_point int,
	created_at datetime,
)
go

create table comments (
	comment_id int primary key identity(1, 1),
	user_id int foreign key references users(user_id),
	idea_id int foreign key references ideas(idea_id),
	comment_content text,
	isAnonymous tinyint,
	isEnabled tinyint,
	status tinyint,
	created_at datetime
)
go
insert into departments (department_name) 
				 values ('Administration'),
						('Quality Assurance'),
						('Computer Science'),
						('Engineering'),
						('Architecture'),
						('Art History')
go
insert into roles (role_name, description) 
		   values ('Admin', null),
				  ('QA Manager', null),
				  ('QA Coordinator', null),
				  ('Staff', null),
				  ('Student', null)
go
insert into users (user_university_id, department_id, role_id, user_name, email, password) 
		   values   /*-----Admin-----*/
					('44444', 1, 1, 'Claire11111', 'ClaireSTU11111@gmail.com', '12345'),

					/*-----QA Manager-----*/
					('55555', 2, 2, 'AchmedSTA55555', 'AchmedSTA55555@gmail.com', '12345'),

					/*-----QA Coordinators-----*/
					('11111', 3, 3, 'DaveSTA11111', 'DaveSTA11111@gmail.com', '12345'),	/*email: DaveSTA11111@gmail.com emailpassword: 123XYZ456 */
					('22222', 4, 3, 'JohnSTA22222', 'JohnSTA22222@gmail.com', '12345'),	/*email: JohnSTA22222@gmail.com emailpassword: 123XYZ456 */
					('66666', 5, 3, 'BrianSTA44444', 'BrianSTA44444@gmail.com', '12345'),
					('10201', 6, 3, 'KeithSTA10201', 'KeithSTA10201@gmail.com', '12345'),

					/*-----Staff-----*/
					('88888', 5, 4, 'AronSTA88888', 'AronSTA88888@gmail.com', '12345'),
					('90849', 3, 4, 'GeorgeSTA90849', 'GeorgeSTA90849@gmail.com', '12345'),

					/*-----Students-----*/
					('33333', 3, 5, 'SarahSTA33333', 'SarahSTA33333@gmail.com', '12345'),	/*email: SarahSTA33333@gmail.com emailpassword: 123XYZ456 */
					('99999', 3, 5, 'AnkeSTU99999', 'AnkeSTU99999@gmail.com', '12345'),
					('10101', 4, 5, 'CharlesSTU10101', 'CharlesSTU10101@gmail.com', '12345'),
					('77777', 5, 5, 'DavidSTU22222', 'DavidSTU22222@gmail.com', '12345'),
					('35473', 6, 5, 'MikeSTU3547', 'MikeSTU35473@gmail.com', '12345')
				
go
insert into categories (category_name) 
				values ('Enrollment'),
					   ('University Premises'),
					   ('Academic Activities'),
					   ('Non-Academic Activities'),
					   ('Courses'),
					   ('Library'),
					   ('Canteen'),
					   ('Other')											  
go
insert into academic_years (academic_year_name, started_at, ended_at, deadline_ideas, deadline_comments)
					values ('2017', '2017-01-14', '2017-12-29', '2017-10-31', '2017-11-30'),
						   ('2018', '2018-01-15', '2018-12-28', '2018-10-31', '2018-11-30'),
						   ('2019', '2019-01-16', '2019-12-27', '2019-10-31', '2019-11-30'),
						   ('2020', '2020-01-17', '2020-12-26', '2020-10-31', '2020-11-30')
go
insert into ideas (user_id, category_id, academic_year_id, idea_title, idea_content, viewed_count, isAnonymous, isEnabled, status, created_at)
		   values (9, 8, 2, 'End-Year Party', 'The end of the last semester is near, but have not seen any information regarding an end-year party. Is there going to be one or is there funds to organize one by the students? I am sure there are many students who are willing to sacrifice their time to organize one.', 0, 1, 1, 1, '2018-01-22'),
				  (12, 3, 2, 'International Field trip', 'Hey guys, I was wondering if there are more people who would love to go on an international field trip once a year? The cost should be taken care of by ourselves, but I think it will be a lot of fun.', 0 , 0, 1 ,1 , '2018-03-05'),
			      (11, 7, 2, 'Better food in the canteen!', 'I am in the second year now and am totally fed up with the food that is served in the canteen. The food is tasteless and seems unhealthy. Is there a change we can improve the food and work on a better health?', 0, 1, 1, 1, '2018-04-12'),
				  (13, 6, 2, 'Library books need an upgrade!', 'Hi all, all the books in the library are in urgent need of an upgrade. I have just recently started my research for a project I am doing, and I came to the conclusion that all book related to my topic are from before the year 2001. It is crucial for my research that I use up-to-date materials, however, I am unable to use the books from the library and am forced to find sources elsewhere. In my opinion we pay enough tuition fee for the university to purchase relevant and current books. What do others think of this?', 0, 0, 1, 1, '2018-04-30'),
				  (10, 1, 2, 'Tedious enrollment process', 'Hi there, i am currently in the process of enrolling to the university and it has been going alright. However, I do find your enrollment process a bit tedious. Is there any change on looking into this matter and make improvements?', 0, 1, 1, 0, '2018-04-30'), 
				  (9, 7, 2, 'Lack of fruits', 'In my opinion, our canteen lacks the proper amount of different fruits. Every day the same fruits are available, however, a diverse eating habit would be better for our health.', 0, 0, 1, 1, '2018-03-21'),
				  (9, 3, 2, 'Swimming contest', 'I think it would be great to organize an annual swimming contest in the our swimming pool. is anyone willing to discuss the possibilities with the swimming club?', 0, 1, 1, 1, '2018-03-30'),
				  (12, 5, 2, 'Off Campus courses', 'I am currently enrolled in a teaching course and is given at an other university during evening classes. I am interested in other evening or off campus classes, however, I cannot find any courses provided by our university. Could it beneficial for our students to have access to these type of courses?', 0, 0, 1, 0, '2018-04-23'),
				  (13, 4, 2, 'Clean up old cinema', 'There is this old cinema right next to our university that needs a big clean up. Are there any volunteers who would like to join us and make a good name for our university?', 0, 0, 1, 1, '2018-03-02'),
				  (9, 4, 2, 'Circus night!', 'Our club want to organize more events, however, it is hard to find any fund. In our opinion, the university should provide fund for clubs so that they can organize events where other students can enjoy and have a great time. Let us not forget that it is not all about studying!', 0, 1, 1, 1, '2018-02-13'),
				  (11, 8, 2, 'Hideous commercial', 'I just would like to express my feeling about the new tv commercial, which they started broadcasting since last week. What a waste of money!', 0, 1, 1, 2, '2018-02-13'),
				  (13, 2, 2, 'Where did all the trash cans go?', 'I am not sure when it happened, but i recently noticed that all trash cans are gone. What does the university want us to do with our trash when all trash cans are taken away from the premises?', 0,0,1,1, '2018-04-30')  

go
insert into comments (user_id, idea_id, comment_content, isAnonymous, isEnabled, status, created_at)
			  values (11, 1, 'I am willing to organize the end-year party!', 0, 1, 1, '2018-01-23'),
					 (7, 1, 'I also have not heard anything yet. Lets organize it before it is too late!', 1, 1, 1, '2018-01-24'),
					 (9, 1, 'Last year it was held in October.', 1, 1, 1, '2018-01-30'),
					 (10, 3, 'The food is not that bad as you suggest. You should see some foods they serve at other universities.', 0, 1, 1, '2018-04-14'),
					 (11, 4, 'I have the same problem. The public library basically has more sources available than in our university.', 0, 1, 1, '2018-04-30'),
					 (8, 3, 'I specifically like the burgers served every Wednesday :)', 0, 1, 1, '2018-04-16'),
					 (9, 3, 'I agree!', 1, 1, 1, '2018-04-16'),
					 (9, 1, 'Sign me up as well in case you need more organizers', 0, 1, 1, '2018-02-15'),
					 (12, 4, 'It all depends on the field you are researching. I never faced any problems, however, I have not had the opportunity to research much.', 1, 1, 1, '2018-04-30'),
					 (10, 4, 'I agree!! Books need to be upgraded!', 0, 1, 1, '2018-05-01'),
					 (7, 10, 'I believe our Architecture department is willing to help with the funds if our students are included in the designing process. Any other departments want to join in?.', 0, 1, 1, '2018-02-14'),
					 (8, 6, 'I have to agree with this student. The same fruit day in, day out is a bit boring and could be improved.', 0, 1, 1, '2018-03-22')
select * from ideas;
delete from ideas where idea_id = 18;