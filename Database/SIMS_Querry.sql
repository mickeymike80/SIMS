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
