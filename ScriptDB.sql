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
    type_label VARCHAR(60)
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
    lable VARCHAR(100) NOT NULL,
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

/*Внесение специалистов и услуг*/

/*Внесение категорий*/
INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Амбулаторная помощь');

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Эндоскопические методы исследования');

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Функциональные методы исследования');

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Ультразвуковые исследования');

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Ренгенологические исследования');

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Офтальмологические исследования');

INSERT INTO `paid_hospital_db`.`type_service`
(`type_label`)
VALUES
('Хирургические вмешательства');

/*Внесение специалистов*/

/*Невролог*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('kosma3261doc'),
'kosma',
md5(4122));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'kosma'),
'Косма Ксения Юрьевна',
2,
'Невролог');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием врача-невролога',
250,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'kosma')),
(SELECT id FROM service WHERE lable = 'Прием врача-невролога'));


/*Офтальмолог*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('zak3261doc'),
'zackurin',
md5(3462));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'zackurin'),
'Закруткин Артём Артемиевич',
1,
'Офтальмолог');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием врача-офтальмолога',
250,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'zackurin')),
(SELECT id FROM service WHERE lable = 'Прием врача-офтальмолога'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Определение характера зрения',
180,
(SELECT id FROM type_service WHERE type_label = 'Офтальмологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'zackurin')),
(SELECT id FROM service WHERE lable = 'Определение характера зрения'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Подбор очковой коррекции зрения',
965,
(SELECT id FROM type_service WHERE type_label = 'Офтальмологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'zackurin')),
(SELECT id FROM service WHERE lable = 'Подбор очковой коррекции зрения'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Определение рефракции с помощью набора пробных линз',
965,
(SELECT id FROM type_service WHERE type_label = 'Офтальмологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'zackurin')),
(SELECT id FROM service WHERE lable = 'Определение рефракции с помощью набора пробных линз'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Измерение угла косоглазия',
110,
(SELECT id FROM type_service WHERE type_label = 'Офтальмологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'zackurin')),
(SELECT id FROM service WHERE lable = 'Измерение угла косоглазия'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Тонометрия глаза',
180,
(SELECT id FROM type_service WHERE type_label = 'Офтальмологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'zackurin')),
(SELECT id FROM service WHERE lable = 'Тонометрия глаза'));

/*Эндокринолог*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('rig3261doc'),
'righova',
md5(5431));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'righova'),
'Рыжова Татьяна Сидоровна',
3,
'Эндокринолог');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием врача-эндокринолога',
250,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'righova')),
(SELECT id FROM service WHERE lable = 'Прием врача-эндокринолога'));

/*Гастроэнтеролог*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('pav3261doc'),
'pavel',
md5(2726));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'pavel'),
'Рящин Павел Леонович',
4,
'Гастроэнтеролог');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием врача-гастроэнтеролог',
350,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'pavel')),
(SELECT id FROM service WHERE lable = 'Прием врача-гастроэнтеролог'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Эзофагогастродуоденоскопия',
2054,
(SELECT id FROM type_service WHERE type_label = 'Эндоскопические методы исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'pavel')),
(SELECT id FROM service WHERE lable = 'Эзофагогастродуоденоскопия'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Ректороманоскопия',
1590,
(SELECT id FROM type_service WHERE type_label = 'Эндоскопические методы исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'pavel')),
(SELECT id FROM service WHERE lable = 'Ректороманоскопия'));

/*Терапевт*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('han3261doc'),
'hanipova',
md5(1428));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'hanipova'),
'Ханипова Наталья Петровна',
6,
'Терапевт');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием врача-терапевта',
250,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'hanipova')),
(SELECT id FROM service WHERE lable = 'Прием врача-терапевта'));

/*Хирург*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('dro3261doc'),
'drozdov',
md5(8941));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'drozdov'),
'Дроздов Давид Зиновиевич',
5,
'Хирург');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием хирурга',
250,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Прием хирурга'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Постановка постоянной трахеостомы',
1663,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Постановка постоянной трахеостомы'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Вазотомия',
1187,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Вазотомия'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Вскрытие паратонзиллярного абсцесса',
2086,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Вскрытие паратонзиллярного абсцесса'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Вскрытие фурункула носа',
1699,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Вскрытие фурункула носа'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Наложение повязки',
638,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Наложение повязки'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Пункция околоносовых пазух',
1861,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Пункция околоносовых пазух'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Трахеотомия',
3348,
(SELECT id FROM type_service WHERE type_label = 'Хирургические вмешательства'),
'Хирургическое вмешательство');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'drozdov')),
(SELECT id FROM service WHERE lable = 'Трахеотомия'));

/*Кардиолог*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('rod3261doc'),
'rodzanko',
md5(2046));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'rodzanko'),
'Родзянко Юлия Карповна',
7,
'Кардиолог');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Прием врача-кардиолога',
350,
(SELECT id FROM type_service WHERE type_label = 'Амбулаторная помощь'),
'Консультация-осмотр');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'rodzanko')),
(SELECT id FROM service WHERE lable = 'Прием врача-кардиолога'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Реоэнцефалография',
960,
(SELECT id FROM type_service WHERE type_label = 'Функциональные методы исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'rodzanko')),
(SELECT id FROM service WHERE lable = 'Реоэнцефалография'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Велоэргометрия',
1890,
(SELECT id FROM type_service WHERE type_label = 'Функциональные методы исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'rodzanko')),
(SELECT id FROM service WHERE lable = 'Велоэргометрия'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Бодиплетизмография',
1363,
(SELECT id FROM type_service WHERE type_label = 'Функциональные методы исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'rodzanko')),
(SELECT id FROM service WHERE lable = 'Бодиплетизмография'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Реовазография',
736,
(SELECT id FROM type_service WHERE type_label = 'Функциональные методы исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'rodzanko')),
(SELECT id FROM service WHERE lable = 'Реовазография'));

/*УЗИ-специалист*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('nov3261doc'),
'novosel',
md5(8248));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'novosel'),
'Новосельцев Тимур Валериевич',
8,
'УЗИ-специалист');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Дуплексное сканирование транскраниальное артерий и вен',
1117,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Дуплексное сканирование транскраниальное артерий и вен'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Ультразвуковое исследование почек',
1215,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Ультразвуковое исследование почек'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Комплекное УЗИ',
1348,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Комплекное УЗИ'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Ультразвуковое исследование молочных желез',
985,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Ультразвуковое исследование молочных желез'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Ультразвуковое исследование органов мошонки',
1325,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Ультразвуковое исследование органов мошонки'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Дуплексное сканирование аорты',
1477,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Дуплексное сканирование аорты'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Ультразвуковое исследование простаты',
1058,
(SELECT id FROM type_service WHERE type_label = 'Ультразвуковые исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'novosel')),
(SELECT id FROM service WHERE lable = 'Ультразвуковое исследование простаты'));

/*Рентгенолог*/
INSERT INTO `paid_hospital_db`.`accounts`
(`id_group`,
`id_user`,
`login`,
`pwd`)
VALUES
('2211',
md5('kyk3261doc'),
'kyklov',
md5(5798));

INSERT INTO `paid_hospital_db`.`doctor`
(`id_user`,
`fio`,
`cabinet_number`,
`specialization`)
VALUES
((SELECT id_user FROM accounts WHERE login = 'kyklov'),
'Куклов Владимир Мечиславович',
9,
'Рентгенолог');

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Рентгенография любой конечности',
488,
(SELECT id FROM type_service WHERE type_label = 'Ренгенологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'kyklov')),
(SELECT id FROM service WHERE lable = 'Рентгенография любой конечности'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Рентгенография легких',
488,
(SELECT id FROM type_service WHERE type_label = 'Ренгенологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'kyklov')),
(SELECT id FROM service WHERE lable = 'Рентгенография легких'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Компьютерная томография грудной аорты',
12863,
(SELECT id FROM type_service WHERE type_label = 'Ренгенологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'kyklov')),
(SELECT id FROM service WHERE lable = 'Компьютерная томография грудной аорты'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Компьютерная томография головы',
2678,
(SELECT id FROM type_service WHERE type_label = 'Ренгенологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'kyklov')),
(SELECT id FROM service WHERE lable = 'Компьютерная томография головы'));

INSERT INTO `paid_hospital_db`.`service`
(`lable`,
`price`,
`id_type`,
`action_service`)
VALUES
('Внутривенная урография',
4300,
(SELECT id FROM type_service WHERE type_label = 'Ренгенологические исследования'),
'Исследование');

INSERT INTO `paid_hospital_db`.`service_of_doctor`
(`id_doctor`,
`id_service`)
VALUES
((SELECT id FROM doctor WHERE id_user = (SELECT id_user FROM accounts WHERE login = 'kyklov')),
(SELECT id FROM service WHERE lable = 'Внутривенная урография'));