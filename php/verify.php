<?php
require 'conn.php'; 

if(isset($_GET['hash_e']) && !empty($_GET['hash_e']))
{
    // Verify data
    $email = mysql_escape_mimic($_GET['hash_e']); // Set email variable
	
	$preparedStatement = dbConnection::get()->prepare('UPDATE user SET email_verified = :email_verified WHERE email_hash = :email LIMIT 1');
	$preparedStatement->execute(array('email' => $email, 'email_verified' => 'true'));
	echo "Email verified";
}

function mysql_escape_mimic($inp) { 
    if(is_array($inp)) 
        return array_map(__METHOD__, $inp); 

    if(!empty($inp) && is_string($inp)) { 
        return str_replace(array('\\', "\0", "\n", "\r", "'", '"', "\x1a"), array('\\\\', '\\0', '\\n', '\\r', "\\'", '\\"', '\\Z'), $inp); 
    } 

    return $inp; 
} 
			
?>
