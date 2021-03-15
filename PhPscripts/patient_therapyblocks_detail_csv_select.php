<?php
include 'connection.php'; 

$patient = $_POST["patient"];

////////////////////////////////////////////////
// retrieve xml history

$strBlockCsvAll = "";
$preparedStatement = $dbConnection->prepare('SELECT block_idx, block_csv, date FROM therapy_block_detail WHERE patient_id = :id_patient ORDER BY block_idx');
$preparedStatement->execute(array('id_patient' => $patient));
while ($row = $preparedStatement -> fetch()) 
{
	// get the data
	$strBlockCsv = $row['block_csv'];
		
	// format csv file
	$strBlockCsvAll = $strBlockCsvAll.$strBlockCsv."\n";	
}

echo $strBlockCsvAll;
?>