CREATE DATABASE paid_hospital_db;
USE paid_hospital_db;

/*
* Таблица с учётными записями работников учереждения
* Содержит в себе: 
* 1) Уникальный индификатор
* 2) Индификатор группы работников(всего две группы)
* 3) Логин
* 4) Пароль
*/
CREATE TABLE accounts(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_group INT UNSIGNED NOT NULL,
    id_user CHAR(32) NOT NULL UNIQUE,
    login VARCHAR(20) NOT NULL UNIQUE,
    pwd CHAR(32) NOT NULL UNIQUE
);

/*
* Таблица работников регистратуры
* Содержит в себе:
* 1) Уникальный индификатор
* 2) Индификатор группы работников(всего две группы, нужен для связи с таблице "Аккаунт")
* 3) ФИО
*/

CREATE TABLE registrator(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_user VARCHAR(100) NOT NULL UNIQUE REFERENCES accounts(id_user) ON DELETE CASCADE,
    fio VARCHAR(60) NOT NULL
);

/*
* Таблица специалистов
* Содержит в себе:
* 1) Уникальный индификатор
* 2) Индификатор группы работников(всего две группы, нужен для связи с таблице "Аккаунт")
* 3) ФИО
* 4) Номер кабинета
* 5) Специализация
*/

CREATE TABLE doctor(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_user VARCHAR(100) NOT NULL UNIQUE REFERENCES accounts(id_user) ON DELETE CASCADE,
    fio VARCHAR(60) NOT NULL,
    cabinet_number INT UNSIGNED NOT NULL,
    specialization VARCHAR(40)
);

/*
* Таблица клиент
* Содержит в себе:
* 1) Уникальный индификатор
* 2) Фамилия
* 3) Имя
* 4) Отчетсво
* 5) Пол
* 6) День рождения
* 7) Адрес проживания
* 8) Телефоный номер
*/

CREATE TABLE patient(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    l_name VARCHAR(45) NOT NULL,
    f_name VARCHAR(45) NOT NULL,
    t_name VARCHAR(45) NOT NULL,
    sex CHAR(3) NOT NULL,
    birthday DATE NOT NULL,
    address VARCHAR(45) NULL,
    phone_number VARCHAR(45) NULL
);

/*
* Таблица тип услуги 
* Содержит в себе:
* 1) Уникальный индификатор
* 2) Тип услуги
*/

CREATE TABLE type_service(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    type_label VARCHAR(40)
);

/*
* Таблица услуги учереждения 
* Содержит в себе:
* 1) Уникальный индификатор
* 2) Название
* 3) Цена
* 4) инификатор типа услуги(нужен для связи с таблице "Тип услуги")
*/

CREATE TABLE service(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    lable VARCHAR(40) NOT NULL,
    price INT UNSIGNED NOT NULL,
    id_type INT UNSIGNED NOT NULL REFERENCES type_service(id),
    action_service VARCHAR(40)
);

/*
* Таблица услуги присущие доктору(Связывает услуги которые оказывает определённый доктор) 
* Содержит в себе:
* 1) Уникальный индификатор
* 2) индификатор специалиста(нужен для связи с таблицей "Специалист")
* 3) индификатор усуги(нужен для связи с таблицей "Услуги учереждения")
*/

CREATE TABLE service_of_doctor(
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_doctor INT UNSIGNED NOT NULL REFERENCES doctor (id),
    id_service INT UNSIGNED NOT NULL REFERENCES service (id)
);

/*
* Таблица записи
* Содержит в себе:
* 1) Уникальный индификатор
* 2) индификатор регистратора(нужен для связи с таблицей "Работник регистратуры")
* 3) дата и время
* 4) индификатор клиента(нужен для связи с таблицей "Клиент")
* 5) индификатор специалиста(нужен для связи с таблицей "Специалист")
* 6) индификатор усуги(нужен для связи с таблицей "Услуги учереждения")
*/

CREATE TABLE entry (
	id INT UNSIGNED NOT NULL PRIMARY KEY AUTO_INCREMENT,
    id_registrator INT UNSIGNED NOT NULL REFERENCES registrator (id),
    data_registration DATETIME NOT NULL,
    id_patient INT UNSIGNED NOT NULL REFERENCES patient (id),
    id_doctor INT UNSIGNED NOT NULL REFERENCES doctor (id),
    id_service INT UNSIGNED NOT NULL REFERENCES service (id)
);

/*Внёс регистратора*/

INSERT INTO accounts(id_group, id_user, login, pwd) VALUES(
	1122, MD5('andrey1623reg'), 'andrey', MD5('1623')
);

INSERT INTO registrator(id_user, fio) VALUES(
	(SELECT id_user FROM accounts WHERE login = 'andrey' AND pwd = MD5(1623)), 'Кедо Андрей Максимович'
);

/*Внёс доктора*/


INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('drozdov3261doc'),
'drozdov',
md5(3261));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'drozdov'),
'Дроздов Давид Зиновиевич',
13,
'Хирург');

/*Внёс услугу*/

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Амбулаторная помощь');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Приём хирурга',
250,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

/*Внёс связь между услугой и специалистом*/

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Приём хирурга'));

/*Внёс пациентов*/

INSERT INTO `paid_hospital_db`.`patient`
(`l_name`,
`f_name`,
`t_name`,
`sex`,
`birthday`,
`address`,
`phone_number`)
VALUES
('Филимонов',
'Лев',
'Андреевич',
'муж',
DATE '1986-08-10',
'ул.Пупкина 31.1',
'89130000000');

INSERT INTO `paid_hospital_db`.`patient`
(`l_name`,
`f_name`,
`t_name`,
`sex`,
`birthday`,
`address`,
`phone_number`)
VALUES
('Сабанцева',
'Ирина',
'Кондратьевна',
'жен',
DATE '1976-10-03',
'ул.Гагарина 180.6',
'89523563070');