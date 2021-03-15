<?php
include 'connection.php';

$patient = $_POST["patient"];
$category = $_POST["category"];
$foil_number = $_POST["foil_number"];
$foil_type = $_POST["foil_type"];
$image_list = $_POST["image_list"];
$accuracy = $_POST["accuracy"];

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
//GET THE ID OF THE LAST BLOCK
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_block WHERE id_therapy_session = :id_therapy_session AND date = (SELECT max(date) FROM therapy_block WHERE id_therapy_session = :id_therapy_session_date)');
$preparedStatement->execute(array('id_therapy_session' => $therapy_session_id, 'id_therapy_session_date' => $therapy_session_id));
$row = $preparedStatement -> fetch();
$therapy_block_id = $row['id_therapy_block'];
//UPDATE THE LAST CHALLENGE
$preparedStatement = $dbConnection->prepare('UPDATE therapy_challenge SET id_challenge_category = :id_challenge_category, foil_number = :foil_number, foil_type = :foil_type, image_list = :image_list, accuracy = :accuracy WHERE id_therapy_block = :id_therapy_block and date = (SELECT max(date) FROM (SELECT date FROM therapy_challenge WHERE id_therapy_block = :id_therapy_block_date) as tiempo)');
$preparedStatement->execute(array('id_challenge_category' => $category, 'foil_number' => $foil_number, 'foil_type' => $foil_type, 'image_list' => $image_list, 'accuracy' => $accuracy, 'id_therapy_block' => $therapy_block_id, 'id_therapy_block_date' => $therapy_block_id ));
?>