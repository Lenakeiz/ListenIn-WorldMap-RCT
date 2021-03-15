<?php
include 'connection.php';

$patient = $_POST["patient"];
$date = $_POST["date"];
$current_date = new DateTime($date);
$therapy_time = $_POST["therapy_time"];

//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
//GET THE id OF THE LAST THERAPY DAILY OF THE APATIENT
$row = $preparedStatement -> fetch();
$therapy_daily_id = $row['id_therapy_daily'];
//GET THE ID OF THE LAST SESSION
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_session WHERE id_therapy_daily = :id_therapy_daily AND date = (SELECT max(date) FROM therapy_session WHERE id_therapy_daily = :id_therapy_daily_date)');
$preparedStatement->execute(array('id_therapy_daily' => $therapy_daily_id, 'id_therapy_daily_date' => $therapy_daily_id));
$row = $preparedStatement -> fetch();
$therapy_session_id = $row['id_therapy_session'];
//INSERT THE BLOCK
$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_block (id_therapy_session, date, therapy_time) VALUES (:id_therapy_session, :date, :therapy_time)');
$preparedStatement->execute(array('id_therapy_session' => $therapy_session_id, 'date' => $current_date->format('Y-m-d H:i:s'), 'therapy_time' => $therapy_time));
?>