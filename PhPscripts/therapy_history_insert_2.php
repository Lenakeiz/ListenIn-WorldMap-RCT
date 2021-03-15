<?php
include 'connection.php';

$patient_id = $_POST["patientid"];
$user_profile = $_POST["profile"];
$therapyblocks_csv = $_POST["tb"];
$cifeatures_history = $_POST["cifHistory"];

$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_history_xml WHERE patient_id = :patient_id');
$preparedStatement->execute(array('patient_id' => $patient_id));
$row = $preparedStatement -> fetch();
if ($row)
{
	// update
	$preparedStatement = $dbConnection->prepare('UPDATE therapy_history_xml SET user_profile = :user_profile, therapyblocks_csv = :therapyblocks_csv, cifeatures_history = :cifeatures_history WHERE patient_id = :patient_id');	
	
	$preparedStatement->execute(array('patient_id' => $patient_id, 'user_profile' => $user_profile, 'therapyblocks_csv' => $therapyblocks_csv, 'cifeatures_history' => $cifeatures_history ));
	
}
else
{
	// insert new
	$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_history_xml (patient_id, user_profile, therapyblocks_csv, cifeatures_history) VALUES (:patient_id, :user_profile, :therapyblocks_csv, :cifeatures_history)');
	
	$preparedStatement->execute(array('patient_id' => $patient_id, 'user_profile' => $user_profile, 'therapyblocks_csv' => $therapyblocks_csv, 'cifeatures_history' => $cifeatures_history ));
	
}


/*
$patient = $_POST["patient"];
$date = $_POST["date"];
$current_date = new DateTime($date);

//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
$last_therapy_id = $row['id_therapy_daily'];
//INSERT THE SESSION
$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_session (id_therapy_daily, date) VALUES (:id_therapy_daily, :date)');
$preparedStatement->execute(array('id_therapy_daily' => $last_therapy_id, 'date' => $current_date->format('Y-m-d H:i:s')));
*/

														
/*
// a seriously minimalistic file upload script that you can build upon

//check if something its being sent to this script
if ($_POST) 
{
	//check if theres a field called action in the sent data
	if ( isset ($_POST['action']) ) 
	{
		//if it indeed theres an field called action. check if its value its level upload
		if($_POST['action'] === 'history upload') 
		{
			//backwards compatible safe check for older php servers, http_post_files is deprecated on newer php servers		
    		/*if(!isset($_FILES)  isset($HTTP_POST_FILES)) 
			{
				$_FILES = $HTTP_POST_FILES;
			}*/
				
			//check if the field file which contains the binary data of the actual file uploaded successfully with no errors, the UPLOAD_ERR_OK means no error and upload was successful		
/*		if ($_FILES['file']['error'] === UPLOAD_ERR_OK)
			{
				//check if the file has a name, in this script it has to have a name to be stored, the file name is sent by unity
				if ($_FILES['file']['name'] !== "")
				{
					//this checks the file mime type, to filter the kind of files you want to accept, this script is configured to accept only xml files, you can edit this one and the unity side to allow your desired file		
					if ($_FILES['file']['type'] === 'text/xml')
            		{
						//INSERT THE SESSION
						$preparedStatement = $dbConnection->prepare('INSERT INTO recsys (xml) VALUES (:xml)');
						$preparedStatement->execute(array('xml' => $_FILES));
														
            			/*
						//construct the final file name path, it depends on how your web hosting has things configured, if its x10 free hosting, just change username to your x10 hosting username
            			//also you can change the levels folder to the name of the folder where you want to upload your files, the folder has to exist prior to using this script, or a error will occur
            			// try using __FILE__ constant to find out what is the full path to this file,google more about it if you are not sure whats that about
        				$uploadfile =  '/home/username/public_html/Levels/' . $_FILES['file']['name'];
        				
        				//once all safety checks are done, you can safely move the file from the temporary location to a public accessible location
						move_uploaded_file($_FILES['file']['tmp_name'], $uploadfile);	
						*/
						
/*            		}//if
							
				}//if
					
			}//if
	
		}//if
		
	}//if	
	
}//if
/*
$patient = $_POST["patient"];
$date = $_POST["date"];
$current_date = new DateTime($date);

//get the last therapy daily
$preparedStatement = $dbConnection->prepare('SELECT * FROM therapy_daily WHERE id_patient = :id_patient and date = (SELECT max(date) FROM therapy_daily WHERE id_patient = :id_patient_date)');
$preparedStatement->execute(array('id_patient' => $patient, 'id_patient_date' => $patient));
//GET THE id OF THE LAST ONE
$row = $preparedStatement -> fetch();
$last_therapy_id = $row['id_therapy_daily'];
//INSERT THE SESSION
$preparedStatement = $dbConnection->prepare('INSERT INTO therapy_session (id_therapy_daily, date) VALUES (:id_therapy_daily, :date)');
$preparedStatement->execute(array('id_therapy_daily' => $last_therapy_id, 'date' => $current_date->format('Y-m-d H:i:s')));
*/
?>