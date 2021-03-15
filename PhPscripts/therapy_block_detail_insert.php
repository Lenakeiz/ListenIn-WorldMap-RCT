<?php
include 'connection.php';

$patient_id = $_POST["patientid"];
$block_idx = $_POST["blockidx"];
$block_csv = $_POST["csv"];
$date = $_POST["date"];


/*$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_history_xml WHERE patient_id = :patient_id');
$preparedStatement->execute(array('patient_id' => $patient_id));
$row = $preparedStatement -> fetch();
if ($row)
{
	// update
	$preparedStatement = $dbConnection->prepare('UPDATE therapy_history_xml SET user_profile = :user_profile, therapyblocks_csv = :therapyblocks_csv, cifeatures_history = :cifeatures_history WHERE patient_id = :patient_id');	
	
	$preparedStatement->execute(array('patient_id' => $patient_id, 'user_profile' => $user_profile, 'therapyblocks_csv' => $therapyblocks_csv, 'cifeatures_history' => $cifeatures_history ));
	
}
else*/
{
	// insert new
	//$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_block_detail (patient_id, block_idx, block_csv) VALUES (:patient_id, :block_idx, :block_csv)');
	
	//$preparedStatement->execute(array('patient_id' => $patient_id, 'block_idx' => $block_idx, 'block_csv' => $block_csv));
	
	$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_block_detail (patient_id, block_idx, block_csv, date) VALUES (:patient_id, :block_idx, :block_csv, :date)');
	
	$preparedStatement->execute(array('patient_id' => $patient_id, 'block_idx' => $block_idx, 'block_csv' => $block_csv, 'date' => $date ));
	
}

?>