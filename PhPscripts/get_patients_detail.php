<?php
include 'connection.php'; 

$result = "";

//ASK FOR THE USER 
$preparedStatement = $dbConnection->prepare('SELECT * FROM patient');

$preparedStatement->execute();

foreach ($preparedStatement as $row) {
	$result .= $row['id_patient'].",";
	$result .= $row['dataset_id'].",";
	$result .= $row['date_added'].",";
}

echo $result;

?>