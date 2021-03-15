<?php
include 'connection.php';

$patient = $_POST["patient"];
$date = $_POST["date"];
$current_date = new DateTime($date);

//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
$last_therapy_id = $row['id_therapy_daily'];
//INSERT THE SESSION
$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_session (id_therapy_daily, date) VALUES (:id_therapy_daily, :date)');
$preparedStatement->execute(array('id_therapy_daily' => $last_therapy_id, 'date' => $current_date->format('Y-m-d H:i:s')));


?>