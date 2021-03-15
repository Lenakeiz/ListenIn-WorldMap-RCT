<?php
$file_name = $_POST['file_name'];
$user_id = $_POST['user_id'];
$folder_route = '../files/'.$user_id;
$directory = '../files/'.$user_id.'/'.$file_name;
$file_game = $_POST['content'];

$checksumPHP = crc32($file_game);
$checksumC = $_POST['checksum'];
$content = "";

if($checksumC == $checksumPHP)
{
	if (!file_exists($folder_route))
	{
		mkdir($folder_route, 0777, true);
	}
	
	if (file_exists($directory))
	{
		$file_server = file_get_contents($directory);
	}
	else 
	{
		$file_server = "";
	}

	//file lenghts FOR CHECKING CONSISTENCY
	$file_game_lenght_diff = strlen($file_game);
	$file_server_lenght_diff = strlen($file_server);
	
	//file arrays
	$file_game_array = explode("\n", $file_game);
	$file_server_array = explode("\n", $file_server);

	//file lenghts
	$file_game_lenght = count($file_game_array);
	$file_server_lenght = count($file_server_array);

	if($file_game_lenght_diff == $file_server_lenght_diff) //up to date
	{
		echo "ok";
		return;
	}
	else if($file_game_lenght_diff < $file_server_lenght_diff) //server newer
	{
		//"server"
		$content = $file_name."#";
		for($i = $file_game_lenght - 1; $i < $file_server_lenght; $i++)
		{
			if(!empty($file_server_array[$i]))
			{
				$content .= $file_server_array[$i];
				
				if($i != $file_server_lenght - 1)
				{
					$content .= "\n";
				}
			}
		}
		echo $content;
		return;
	}
	else if($file_game_lenght_diff > $file_server_lenght_diff) //game newer
	{
		//"local"
		for($i = $file_server_lenght - 1; $i < $file_game_lenght; $i++)
		{
			if(!empty($file_game_array[$i]))
			{
				$content .= $file_game_array[$i];
				
				if($i != $file_game_lenght - 1)
				{
					$content .= "\n";
				}
			}
		}
		file_put_contents($directory, $content, FILE_APPEND | LOCK_EX);
		echo "local";
		return;
	}
}
else
{
	echo "checksum_error";
}
?>
