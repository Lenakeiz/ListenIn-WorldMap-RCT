<?php
include 'connection.php';

$patient = $_POST["patient"];
$cif_idx = $_POST["cif_idx"];
$stim_ori_idx = $_POST["stim_ori_idx"];
$stim_type = $_POST["stim_type"];
$selected_stim_idx = $_POST["selected_stim_idx"];
$rt_sec = $_POST["rt_sec"];
/*$category = $_POST["category"];
$foil_number = $_POST["foil_number"];
$foil_type = $_POST["foil_type"];
$image_list = $_POST["image_list"];*/
$accuracy = $_POST["accuracy"];
$date = $_POST["date"];
$current_date = new DateTime($date);

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
//INSERT THE CHALLENGE
$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_challenge (id_therapy_block, cif_idx, stim_ori_idx, stim_type, selected_stim_idx, rt_sec, accuracy, date) VALUES (:id_therapy_block, :cif_idx, :stim_ori_idx, :stim_type, :selected_stim_idx, :rt_sec, :accuracy, :date)');
$preparedStatement->execute(array('id_therapy_block' => $therapy_block_id, 'cif_idx' => $cif_idx, 'stim_ori_idx' => $stim_ori_idx, 'stim_type' => $stim_type, 'selected_stim_idx' => $selected_stim_idx, 'rt_sec' => $rt_sec, 'accuracy' => $accuracy, 'date' => $current_date->format('Y-m-d H:i:s')));
/*$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_challenge (id_therapy_block, id_challenge_category, foil_number, foil_type, accuracy, image_list, date) VALUES (:id_therapy_block, :id_challenge_category, :foil_number, :foil_type, :accuracy, :image_list, :date)');
$preparedStatement->execute(array('id_therapy_block' => $therapy_block_id, 'id_challenge_category' => $category, 'foil_number' => $foil_number, 'foil_type' => $foil_type, 'accuracy' => $accuracy, 'image_list' => $image_list, 'date' => $current_date->format('Y-m-d H:i:s')));*/
?>