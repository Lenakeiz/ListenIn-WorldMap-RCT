<?php
include 'connection.php'; 

$patient = $_POST["patient"];
$progress = $_POST["progress"];

$preparedStatement = $dbConnection->prepare('SELECT * FROM patient_game WHERE id_patient = :id_patient');
$preparedStatement->execute(array('id_patient' => $patient));
$row = $preparedStatement -> fetch();
if ($row)
{
	//UPDATE THE GAME PROGRESS OF THE PATIENT
	$preparedStatement = $dbConnection->prepare('UPDATE patient_game SET game_progress = :game_progress WHERE id_patient = :id_patient');
	$preparedStatement->execute(array('game_progress' => $progress, 'id_patient' => $patient));
}
else
{
	// insert new
	$preparedStatement = $dbConnection->prepare('INSERT INTO patient_game (id_patient, game_progress) VALUES (:id_patient, :game_progress)');
	
	$preparedStatement->execute(array('id_patient' => $patient, 'game_progress' => $progress));
}
?>