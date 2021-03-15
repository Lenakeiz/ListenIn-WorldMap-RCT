<?php
include 'connection.php'; 

$result = "";

//ASK FOR THE USER 
$preparedStatement = $dbConnection->prepare('SELECT * FROM patient');

$preparedStatement->execute();

foreach ($preparedStatement as $row) {
	$result .= $row['id_patient'].",";
}

echo $result;

?>