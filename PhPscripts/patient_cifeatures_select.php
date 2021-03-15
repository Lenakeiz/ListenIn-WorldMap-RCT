<?php
include 'connection.php'; 

$patient = $_POST["patient"];
$user_profile = "";
$therapyblocks = "";
$cifeatures_history = "";

////////////////////////////////////////////////
// retrieve xml history
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_history_xml WHERE patient_id = :id_patient');
$preparedStatement->execute(array('id_patient' => $patient));
$row = $preparedStatement -> fetch();
$user_profile = $row['user_profile'];
$therapyblocks = $row['therapyblocks'];
$cifeatures_history = $row['cifeatures_history'];

echo $cifeatures_history;
?>