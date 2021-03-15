<?php
include 'connection.php'; 

$patient_id = $_POST["patientid"];
$dataset = $_POST["dataset"];
$start_date = $_POST["date"];

//patient exist?
$preparedStatement = $dbConnection->prepare('SELECT * FROM patient WHERE id_patient = :id_patient');
$preparedStatement->execute(array('id_patient' => $patient_id));

//if it doesn't
if($preparedStatement->rowCount() == 0)
{
	//insert patient
	$preparedStatement = $dbConnection->prepare('INSERT INTO patient (id_patient, dataset_id, date_added) VALUES (:id_patient, :dataset_id, :date_added)');
	$preparedStatement->execute(array('id_patient' => $patient_id, 'dataset_id' => $dataset, 'date_added' => $start_date ));	
}
else
{
	echo "0";
}

echo "0";
	

?>