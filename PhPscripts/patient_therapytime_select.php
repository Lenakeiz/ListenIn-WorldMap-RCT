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

////////////////////////////////////////////////
// retrieve game time
$strGameTime = "<game_time>\n";
//$strQuery = sprintf("SELECT date, today_game_time_min FROM game_time WHERE patient_id=%s ORDER BY date DESC", GetSQLValueString($patient, "text");
//mysql_select_db($database_connEyeSearch, $connEyeSearch);
//$rsResult = mysql_query($strQuery, $connEyeSearch);
$preparedStatement = $dbConnection->prepare('SELECT date, today_game_time_min FROM game_time WHERE patient_id = :id_patient ORDER BY date DESC');
$preparedStatement->execute(array('id_patient' => $patient));
//while ($row = mysql_fetch_assoc($rsResult)) 
while ($row = $preparedStatement -> fetch()) 
{
	// get the data
	$strDate = $row['date'];
	$strTodayGameTimeMin = $row['today_game_time_min'];	
	
	// format xml data
	$strGameTime = $strGameTime."<gt date=\"".$strDate."\" min=\"".$strTodayGameTimeMin."\"></gt>\n";	

}
$strGameTime = $strGameTime."</game_time>\n";

////////////////////////////////////////////////
// retrieve the last therapy time
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_time WHERE patient_id = :id_patient and date = (SELECT max(date) FROM therapy_time WHERE patient_id = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
$row = $preparedStatement -> fetch();
$totalTherapyTimeMin = $row['total_therapy_time_min'];

// format xml data to return
$strData = "<data>\n";
$strContent = "<user_profile>".$user_profile."</user_profile>\n";
//$strData = $strData.$strContent;
$strContent = "<therapyblocks>".$therapyblocks."</therapyblocks>\n";
//$strData = $strData.$strContent;
$strContent = "<cifeatures_history>".$cifeatures_history."</cifeatures_history>\n";
//$strData = $strData.$strContent;
$strContent = "<therapyTime>".$totalTherapyTimeMin."</therapyTime>\n";
$strData = $strData.$strContent;
$strData = $strData.$strGameTime;
$strData = $strData."</data>";
echo $strData;

?>