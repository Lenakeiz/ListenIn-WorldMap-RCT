<?php
include 'connection.php'; 

$patient = $_POST["patient"];
//ONLINE WILL GET THE CURRENT DATETIME
$current_date = new DateTime('now',new DateTimeZone('Europe/London'));

//patient exist?
$preparedStatement = $dbConnection->prepare('SELECT * FROM patient WHERE id_patient = :id_patient');
$preparedStatement->execute(array('id_patient' => $patient));

//if it doesn't
if($preparedStatement->rowCount() == 0)
{
	//insert patient
	$preparedStatement = $dbConnection->prepare('INSERT INTO patient (id_patient, date_added) VALUES (:id_patient, :date_added)');
	$preparedStatement->execute(array('id_patient' => $patient, 'date_added' => $current_date->format('Y-m-d H:i:s')));
	
	//insert patient_game, game related table
	$preparedStatement = $dbConnection->prepare('INSERT INTO patient_game (id_patient) VALUES (:id_patient)');
	$preparedStatement->execute(array('id_patient' => $patient));
}
else
{
	echo "0";
}
	

?>