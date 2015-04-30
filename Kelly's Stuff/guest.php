  <?php
$servername = "localhost";
$username = "root";
$password = "Group5";
$dbname = "Creechky";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
     die("Connection failed: " . $conn->connect_error);
} 

$sql = "SELECT UID, Name, Phone_number, FaceFront, Restriction_type FROM user";
$result = $conn->query($sql);
//$UesrID = $row['UID'];
$image = -1;
$decoded_image=base64_decode($image);

echo "<form id='frmOne'>";
echo"<table> <tr>
    <th>Add</th>
    <th>Name</th>
	<th>Phone Number</th>
	<th>Image</th>
	<th>Type</th>
  </tr>";

if ($result->num_rows > 0) {
     // output data of each row
         while($row = $result->fetch_assoc()) {
		$image = $row['FaceFront'];
         echo "<tr> <td> <a href='#UpdateInfo' id='{$row['UID']}' onclick='AddUpdateGuest(this.id)'> <input type='radio'  name='all' id='{$row['UID']}'></a> </td><td> ". $row["Name"]. "</td>
		 <td> " . $row["Phone_number"] . "</td> "  . " <td>";
		 echo '<img src="data:image/jpeg;base64,'.base64_encode($image) .'" />';
		 
		 echo "</td><td> " . $row["Restriction_type"] ."</td> </tr>";
     } 
} else {
     echo "0 results";
}
echo "</table></form>";
$conn->close();
?>  