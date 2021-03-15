<?php
include 'connection.php'; 

$result = "";

//ASK FOR THE SITES
$preparedStatement = $dbConnection->prepare('SELECT * FROM study_site');

$preparedStatement->execute();

foreach ($preparedStatement as $row) {
	$result .= $row['study_site'].",";
}

$result .= ";";

//ASK FOR THE STROKES
$preparedStatement = $dbConnection->prepare('SELECT * FROM type_of_stroke');

$preparedStatement->execute();

foreach ($preparedStatement as $row) {
	$result .= $row['type_of_stroke'].",";
}

echo $result;

?>