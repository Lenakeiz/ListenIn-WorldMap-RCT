<?php
include 'connection.php';

$patient = $_POST["patient"];
$therapy_time = $_POST["therapy_time"];
$therapy_game_pinball = $_POST["therapy_game_pinball"];
$therapy_game_world = $_POST["therapy_game_world"];
$exit_reason = $_POST["exit_reason"];

//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
$last_therapy_id = $row['id_therapy_daily'];
//GET THE CURRENT TIMES
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_session WHERE id_therapy_daily = :id_therapy_daily and date = (SELECT max(date) FROM (SELECT date FROM therapy_session WHERE id_therapy_daily = :id_therapy_daily_date) as tiempo)');
$preparedStatement->execute(array('id_therapy_daily' => $last_therapy_id, 'id_therapy_daily_date' => $last_therapy_id));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
$therapy_time += $row['therapy_time'];
$therapy_game_pinball += $row['therapy_game_pinball'];
$therapy_game_world += $row['therapy_game_world'];
//UPDATE THE LAST SESSION WITH THE TIMES
$preparedStatement = $dbConnection->prepare('UPDATE therapy_session SET therapy_time = :therapy_time, therapy_game_pinball = :therapy_game_pinball, therapy_game_world = :therapy_game_world, id_exit_reason = :id_exit_reason WHERE id_therapy_daily = :id_therapy_daily and date = (SELECT max(date) FROM (SELECT date FROM therapy_session WHERE id_therapy_daily = :id_therapy_daily_date) as tiempo)');
$preparedStatement->execute(array('therapy_time' => $therapy_time, 'therapy_game_pinball' => $therapy_game_pinball, 'therapy_game_world' => $therapy_game_world, 'id_exit_reason' =>  $exit_reason, 'id_therapy_daily' => $last_therapy_id, 'id_therapy_daily_date' => $last_therapy_id));

?>