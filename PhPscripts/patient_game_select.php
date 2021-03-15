<?php
include 'connection.php'; 

$patient = $_POST["patient"];
$result = "";

//ASK FOR THE PROGRESS
$preparedStatement = $dbConnection->prepare('SELECT * FROM patient_game WHERE id_patient = :id_patient');

$preparedStatement->execute(array('id_patient' => $patient));

$row = $preparedStatement -> fetch();
$result = $row['game_progress'];

echo $result;

?>