<?php
include 'connection.php';

$patient = $_POST["patient"];
$level_end = $_POST["level_end"];
$total_therapy_time = $_POST["total_therapy_time"];
//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
$last_therapy_id = $row['id_therapy_daily'];
//UPDATE THE LAST ONE
$preparedStatement = $dbConnection->prepare('UPDATE therapy_daily SET game_level_end = :game_level_end, total_therapy_time = :total_therapy_time WHERE id_therapy_daily = :id_therapy_daily ');
$preparedStatement->execute(array('game_level_end' => $level_end, 'total_therapy_time' => $total_therapy_time, 'id_therapy_daily' => $last_therapy_id));

?>