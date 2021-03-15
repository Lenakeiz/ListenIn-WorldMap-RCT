<?php
require 'conn.php'; 

$email_hash = $_POST["email_hash"];
$email = $_POST["email"];

//ASK FOR ALREADY USED EMAIL
$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user WHERE email_hash = :email_hash');

$preparedStatement->execute(array('email_hash' => $email_hash));

if($preparedStatement->rowCount() > 0)
{
	$to      = $email; // Send email to our user
	$subject = "ListenIn - Change password"; // Give the email a subject
 
	$message = "Please click this link to reset your password:" . "\r\n";
	$message .= "https://listeninsoftv.ucl.ac.uk/php/reset_password_form.php?eh=" . $email_hash . "&e=" . $email . "\r\n";
	$message = nl2br($message);
						 
	$headers = "MIME-Version: 1.0" . "\r\n";
	$headers .= "Content-type:text/html;charset=UTF-8" . "\r\n";
	$headers .= 'From: <noreply@ucl.ac.uk>' . "\r\n";

	mail($to, $subject, $message, $headers);

	echo "bien";
}
else
{
	echo "mal";
}
			
?>
