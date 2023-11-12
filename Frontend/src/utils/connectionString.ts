import { connectionString } from '../config';

const getConnection = (value: string): string => `${connectionString}${value}`;

export { getConnection };
