import express from 'express';
import bodyParser from 'body-parser';
import cors from 'cors';
import boardRoutes from './routes/boardRoutes';

const app = express();
const PORT = process.env.PORT || 5000;

app.use(cors());
app.use(bodyParser.json());
app.use('/api/boards', boardRoutes);

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});

export default app;