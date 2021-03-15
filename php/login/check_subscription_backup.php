<?php
require '../conn.php'; 

$id_user = $_POST["id_user"];
$current_time = date_create(date('Y-m-d h:i:s', time()));

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user WHERE id = :id_user LIMIT 1');

$preparedStatement->execute(array('id_user' => $id_user));

if($preparedStatement->rowCount() > 0)
{
	//CHECK IF A CODE HAS BEEN ASSIGNED TO THE USER
	$preparedStatementCode = dbConnection::get()->prepare('SELECT * FROM redeem_code WHERE id_user_assigned = :id_user LIMIT 1');
	$preparedStatementCode->execute(array('id_user' => $id_user));
	
	if($preparedStatementCode->rowCount() > 0) //IF YES, THEN LET THEM PLAY
	{
		echo "true";
	}
	else //ELSE CHECK SUBSCRIPTION DATE
	{
		$result = $preparedStatement->fetch();
	
		if($result["has_logged"] == 1) //IF THE USER HAS LOGGED ONCE BEFORE, CHECK DATES
		{
			$subscription_time = date_create($result["subscription_end"]);
		
			$time = s_datediff("d", $current_time, $subscription_time, true);
			
			if($time >= 0)
			{
				echo "true";
			}
			else
			{
				echo "false";
			}
		}
		else // ELSE ADD THE 7 FREE DAYS
		{
			$current_time = $current_time->format('Y-m-d h:i:s');
			$subscription = date('Y-m-d h:i:s', strtotime($current_time . ' + 7 days'));
			
			$preparedStatement = dbConnection::get()->prepare('UPDATE user SET subscription_end = :subscription_end, has_logged = 1 WHERE id = :id_user LIMIT 1');
			$preparedStatement->execute(array('subscription_end' => $subscription, 'id_user' => $id_user));
			echo "true";
		}
	}
}
else
{
	echo "error";
}


function s_datediff( $str_interval, $dt_menor, $dt_maior, $relative=false){

       if( is_string( $dt_menor)) $dt_menor = date_create( $dt_menor);
       if( is_string( $dt_maior)) $dt_maior = date_create( $dt_maior);

       $diff = date_diff( $dt_menor, $dt_maior, ! $relative);
       
       switch( $str_interval){
           case "y": 
               $total = $diff->y + $diff->m / 12 + $diff->d / 365.25; break;
           case "m":
               $total= $diff->y * 12 + $diff->m + $diff->d/30 + $diff->h / 24;
               break;
           case "d":
               $total = $diff->y * 365.25 + $diff->m * 30 + $diff->d + $diff->h/24 + $diff->i / 60;
               break;
           case "h": 
               $total = ($diff->y * 365.25 + $diff->m * 30 + $diff->d) * 24 + $diff->h + $diff->i/60;
               break;
           case "i": 
               $total = (($diff->y * 365.25 + $diff->m * 30 + $diff->d) * 24 + $diff->h) * 60 + $diff->i + $diff->s/60;
               break;
           case "s": 
               $total = ((($diff->y * 365.25 + $diff->m * 30 + $diff->d) * 24 + $diff->h) * 60 + $diff->i)*60 + $diff->s;
               break;
          }
       if( $diff->invert)
               return -1 * $total;
       else    return $total;
   }
?>
