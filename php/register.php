<?php
require 'conn.php'; 

$email = $_POST["email"];
$email_hash = $_POST["email_hash"];
$email_encrypted = $_POST["email_encrypted"];
$password = $_POST["password"];
$password_hash = $_POST["password_hash"];
$genre = $_POST["genre"];
$cause = $_POST["cause"];
$date_of_onset = $_POST["date_of_onset"];
$concent = $_POST["concent"];
$can_contact = $_POST["can_contact"];

//ASK FOR ALREADY USED EMAIL
$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user WHERE email_hash = :email_hash');

$preparedStatement->execute(array('email_hash' => $email_hash));

if($preparedStatement->rowCount() > 0)
{
	echo "used";
}
else
{
	$preparedStatement = dbConnection::get()->prepare('INSERT INTO user (email_hash, email_encrypted, password, genre, cause, date_of_onset, concent, can_contact) VALUES (:email_hash, :email_encrypted, :password, :genre, :cause, :date_of_onset, :concent, :can_contact)');
	$preparedStatement->execute(array(
			'email_hash' => $email_hash,
			'email_encrypted' => $email_encrypted,
			'password' => $password_hash,
			'genre' => $genre,
			'cause' => $cause,
			'date_of_onset' => $date_of_onset,
			'concent' => $concent,
			'can_contact' => $can_contact
		));

	$to      = $email; // Send email to our user
	$subject = "ListenIn - Signup | Verification"; // Give the email a subject 
	
	$message = "Thank you for signing up to Listen-In!" . "\r\n";
	$message = "Please go to the link below to activate your account:" . "\r\n";
	$message .= "https://listeninsoftv.ucl.ac.uk/php/verify.php?hash_e=" . $email_hash . "\r\n\n";
	
	$message .= "After this, you can log in to Listen-In with the following credentials:" . "\r\n\n";
	$message .= "-----------------------" . "\r\n";
	$message .= "Mail: " . $email . "\r\n";
	$message .= "Password: " . $password . "\r\n";
	$message .= "------------------------" . "\r\n\n";
	$message .= "If you experience any problems, please contact us at:" . "\r\n";
	$message .= "Listen-in@ucl.ac.uk" . "\r\n";
	$message .= "(Please note: It may take a few days to receive a reply)" . "\r\n";
	$message = nl2br($message);
						 
	$headers = "MIME-Version: 1.0" . "\r\n";
	$headers .= "Content-type:text/html;charset=UTF-8" . "\r\n";
	$headers .= 'From: <noreply@ucl.ac.uk>' . "\r\n";

	mail($to, $subject, $message, $headers);

	echo "bien";
}
			
?>
