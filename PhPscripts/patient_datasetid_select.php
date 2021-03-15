<?php
include 'connection.php'; 

$patient = $_POST["patient"];
$result = "";

//ASK FOR THE PROGRESS
$preparedStatement = $dbConnection->prepare('SELECT * FROM patient WHERE id_patient = :id_patient');

$preparedStatement->execute(array('id_patient' => $patient));

$row = $preparedStatement -> fetch();
$result = $row['dataset_id'];

echo $result;

?>